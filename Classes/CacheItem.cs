using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace CryptLink {
    public class CacheItem {

        [BsonId, BsonIndex]
        public byte[] Key { get; set; }
        public IHashable Value { get; set; } = null;
        public long MemoryHits { get; set; }
        public long DiskHits { get; set; }
        public DateTime LastPersistantSave { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime LastHit { get; set; }

        public CacheItem() { }

        public CacheItem(CByte DesiredLookupKey, IHashable CacheValue, TimeSpan ExpireIn) {
            Key = DesiredLookupKey.Bytes;
            Value = CacheValue;
            ExpireDate = DateTime.Now.Add(ExpireIn);
            AddedDate = DateTime.Now;
        }

        public CByte GetKeyCByte() {
            return Hash.FromComputedBytes(Key, Value.Hash.Provider, 0);
        }

        /// <summary>
        /// An estimate of how desirable this object is to be cached
        /// </summary>
        public double ItemCacheScore() {
            
            if (DateTime.Now > ExpireDate) {
                return 0;
            }

            double hourMax = 12;
            double sizeBuinaryMax = 1048576;
            double hphMax = 1024;

            double totalHitsScore = 1 - Utility.GetRange((MemoryHits + DiskHits) / (DateTime.Now - AddedDate).TotalHours, hphMax); //25% of score
            double lastHitScore = 1 - Utility.GetRange((DateTime.Now - LastHit).TotalHours, hourMax); //50% of score
            double sizeScore = Utility.GetRange(Value.Hash.SourceByteLength, sizeBuinaryMax); //25% of score

            return (totalHitsScore * 0.25) + (lastHitScore * 0.5) + (sizeScore * 0.25);
            
        }



    }
}
