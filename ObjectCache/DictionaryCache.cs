using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;

namespace CryptLink {
    public class DictionaryCache : ObjectCache {
        ConcurrentDictionary<ComparableBytesAbstract, CacheItem> Cache;

        public override bool CacheIsPersistent {
            get {
                return false;
            }
        }

        public override void Initialize() {
            Cache = new ConcurrentDictionary<ComparableBytesAbstract, CacheItem>();
        }

        public override bool CacheIsInitalized {
            get {
                return Cache == null;
            }
        }

        private long _currentCollectionCount;
        public override long CurrentCollectionCount {
            get {
                //return Cache.Count();
                return _currentCollectionCount;
            }
        }

        public override bool AddOrUpdate<T>(ComparableBytesAbstract Key, T Value, TimeSpan ExpireSpan) {
            CountWrite();

            CacheItem newItem = new CacheItem(Key, Value, ExpireSpan);

            return AddOrUpdate(newItem);
        }

        public override bool AddOrUpdate(CacheItem Item) {
            if (!AcceptingObjects) {
                return false;
            }

            bool itemExists = false;
            long oldSize = 0;

            Cache.AddOrUpdate(Item.GetKeyCByte(), Item, (key, existingVal) => {
                //If the Key exists, update the value but keep the metadata
                itemExists = true;
                oldSize = existingVal.Value.Hash.SourceByteLength;
                Item.MemoryHits = existingVal.MemoryHits;
                Item.DiskHits = existingVal.DiskHits;
                Item.LastHit = DateTime.Now;
                return Item;
            });

            if (itemExists) {
                //item exists, subtract the old size and add the new size
                CurrentCollectionSize -= oldSize;
                CurrentCollectionSize += Item.Value.Hash.SourceByteLength;
            } else {
                //item did not exist
                _currentCollectionCount += 1;
                CurrentCollectionSize += Item.Value.Hash.SourceByteLength;
            }

            return true;
        }

        public override void Dispose() {
            Cache = null;
        }

        public override bool Exists(ComparableBytesAbstract Key) {
            CountRead();
            return Cache.ContainsKey(Key);
        }

        public override CacheItem Get(ComparableBytesAbstract Key) {
            CountRead();
            var item = new CacheItem();
            Cache.TryGetValue(Key, out item);
            return item;
        }

        public override T Get<T>(ComparableBytesAbstract Key) {
            var item = Get(Key);
            return (T)item.Value;
        }

        public override bool Expire(DateTime ExpiredAfter) {
            var oldObjects = (from o in Cache where o.Value.ExpireDate < ExpiredAfter select o);

            foreach (var h in oldObjects) {
                var cb = Hash.FromComputedBytes(h.Key.Bytes, h.Value.Value.Provider, h.Value.Value.Hash.SourceByteLength);
                Remove(cb);
            }

            return true;
        }

        public override bool Remove(ComparableBytesAbstract Key) {
            CountWrite();
            var c = new CacheItem();
            Cache.TryRemove(Key, out c);

            if (c?.Value != null) {
                _currentCollectionCount -= 1;
                CurrentCollectionSize -= c.Value.Hash.SourceByteLength;
                return true;
            } else {
                return false;
            }
        }

        public override ComparableBytesAbstract[] GetMigrationCanidates(int Count) { 
            return (from i in Cache orderby i.Value.ItemCacheScore() ascending select i.Value.GetKeyCByte()).Take(Count).ToArray();
        }

        public override void Clear() {
            Cache.Clear();
        }
    }


}

