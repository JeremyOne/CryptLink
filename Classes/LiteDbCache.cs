using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace CryptLink {
    public class LiteDbCache : ObjectCache {
        private LiteDB.LiteDatabase DataBase;
        private LiteDB.LiteCollection<CacheItem> LCol;

        public override bool CacheIsPersistent {
            get {
                return true;
            }
        }

        public override long CurrentCollectionCount {
            get {
                return LCol.Count();
            }
        }

        public override void Initialize() {
            DataBase = new LiteDatabase(ConnectionString);
            LCol = DataBase.GetCollection<CacheItem>("cache");
            LCol.EnsureIndex(c => c.Key);
            //CurrentCollectionCount = LCol.Count();
        }

        public override bool AddOrUpdate<T>(CByte Key, T Value, TimeSpan ExpireSpan) {
            CountWrite();
            CacheItem newItem = new CacheItem(Key, Value, ExpireSpan);
            return AddOrUpdate(newItem);
        }

        public override bool AddOrUpdate(CacheItem Item) {
            if (!AcceptingObjects) {
                return false;
            }

            LCol.Upsert(Item);

            return true;
        }

        public override void Dispose() {
            DataBase.Dispose();
        }

        public override bool Exists(CByte Key) {
            CountRead();
            return LCol.Exists(x => x.Key == Key.Bytes);
        }

        public override CacheItem Get(CByte Key) {
            CountRead();
            return LCol.FindOne(x => x.Key == Key.Bytes);
        }

        public override T Get<T>(CByte Key) {
            var item = Get(Key);
            return (T)item.Value;
        }

        public override bool Expire(DateTime ExpiredAfter) {
            var removedCount = LCol.Delete(x => x.ExpireDate < ExpiredAfter);
            //CurrentCollectionCount -= removedCount;
            return true;
        }

        public override bool Remove(CByte Key) {
            CountWrite();
            var deletes = LCol.Delete(x => x.Key == Key.Bytes);

            if (deletes > 0) {
                //CurrentCollectionCount -= deletes;
                return true;
            } else {
                return false;
            }
        }

        public override CByte[] GetMigrationCanidates(int Count) {
            var canidates = LCol.FindAll().Take(Count);
            return canidates.Select(x => x.GetKeyCByte()).ToArray();
        }
    }
}
