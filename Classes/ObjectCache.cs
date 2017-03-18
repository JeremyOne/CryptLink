using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using System.Collections.Concurrent;
using System.Collections;

namespace CryptLink {
    public abstract class ObjectCache : IObjectCache {

        public bool AcceptingObjects { get; set; }
        public abstract bool CacheIsPersistent { get; }
        public string ConnectionString { get; set; }

        public long MaxCollectionCount { get; set; }
        public long MaxCollectionSize { get; set; }
        public TimeSpan MaxExpiration { get; set; }
        public long MaxObjectSize { get; set; }

        public long MinCollectionSize { get; set; }
        public TimeSpan MinExpiration { get; set; }
        public long MinObjectSize { get; set; }

        public long CurrentCollectionCount { get; set; }
        public long CurrentCollectionSize { get; set; }
        public long CurrentWriteCount { get; set; }
        public long CurrentReadCount { get; set; }

        public TimeSpan ManageEvery { get; set; }
        public int ManageEveryIOCount { get; set; }
        public long ManagedAtIOCount { get; set; }
        public DateTime ManagedLastAt { get; set; }
        public bool ManagementRunning { get; set; }

        public DateTime StartedTime { get; } = DateTime.Now;

        public double CachePreference { get; set; }

        public IObjectCache OverflowCache { get; set; }

        public abstract bool AddOrUpdate<T>(CByte Key, T Value, TimeSpan ExpireSpan) where T : Hashable;
        public abstract bool AddOrUpdate(CacheItem Value);
        public abstract bool Exists(CByte Key);
        public abstract T Get<T>(CByte Key) where T : Hashable;
        public abstract CacheItem Get(CByte Key);
        public abstract bool Remove(CByte Key);
        public abstract bool Expire(DateTime ExpiredAfter);

        public abstract void Dispose();
        public abstract void Initialize();

        /// <summary>
        /// Checks if management is needed, and runs it if needed
        /// </summary>
        public void ManageCheck() {
            bool managementNeeded = false;

            if (ManageEvery.TotalSeconds > 0) {
                if (ManagedLastAt.Add(ManageEvery) < DateTime.Now) {
                    managementNeeded = true;
                }
            }

            if (ManageEveryIOCount > 0) {
                if (CurrentWriteCount > (ManageEveryIOCount + ManagedAtIOCount)) {
                    managementNeeded = true;
                }
            }

            if (managementNeeded) {
                ManagedLastAt = DateTime.Now;
                ManagedAtIOCount = CurrentWriteCount;
                ManageNow();
            } else {
                AcceptingObjects = true;
            }
        }

        public void ManageNow() {
            if (ManagementRunning) {
                return;
            }

            ManagementRunning = true;

            Expire(DateTime.Now);

            if (UseagePercent > 0.75) {
                //migrate objects if we can
                if (OverflowCache != null) {
                    //move 10% of max
                    int numToMove = (int)(MaxCollectionCount * 0.1);
                    CByte[] moveObjects = GetMigrationCanidates(numToMove);
                    MigrateObjects(this, OverflowCache, moveObjects, true);
                } else {
                    //must stop accepting items
                    AcceptingObjects = false;
                }
            }

            ManagementRunning = false;
        }

        /// <summary>
        /// Gets items to migrate to another cache if needed, it's up to the implementation to decide how to choose them
        /// </summary>
        public abstract CByte[] GetMigrationCanidates(int Count);

        public void CountRead() {
            CurrentReadCount += 1;
        }

        public void CountWrite() {
            CurrentWriteCount += 1;
            ManageCheck();
        }

        /// <summary>
        /// Returns a resource usage estimate based on current size and count, where size and count are 50% of total usage.
        /// Note: This estimate will vary in accuracy based on implementation
        /// </summary>
        public double UseagePercent {
            get {
                double count = UseageCountPercent;
                double size = UseageSizePercent;
                return (count * 0.5) + (size * 0.5);
            }
        }

        public double UseageCountPercent {
            get {
                return ((double)CurrentCollectionCount / (double)MaxCollectionCount);
            }
        }

        public double UseageSizePercent {
            get {
                return ((double)CurrentCollectionSize / (double)MaxCollectionSize);
            }
        }

        public double TotalAverageIOPS() {
            long ioTotal = CurrentReadCount + CurrentWriteCount;
            return ioTotal / (DateTime.Now - StartedTime).TotalSeconds;
        }

        /// <summary>
        /// Computes how well this object fits in this implementation of the cache based on:
        /// usage (40%), object size (30%), expiration time (30%) plus cache preference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ForObject"></param>
        /// <returns></returns>
        public double ComputeFitScore(CacheItem ForObject) {
            double score = 1;

            long itemSize = ForObject.Value.Hash.SourceByteLength;
            if ((itemSize > MaxObjectSize) || (itemSize < MinObjectSize)) {
                score -= 0.3;
            }

            var timeTimeToLive = DateTime.Now - ForObject.ExpireDate;
            if ((timeTimeToLive > MaxExpiration) || (timeTimeToLive < MinExpiration)) {
                score -= 0.3;
            }

            score -= UseagePercent / 0.4;
            score += CachePreference;

            return score;
        }

        /// <summary>
        /// Moves objects from one cache to another
        /// </summary>
        public void MigrateObjects(IObjectCache Source, IObjectCache Destination, CByte[] ObjectsToMove, bool RemoveSourceObject) {

            foreach (Hash key in ObjectsToMove) {
                var item = Source.Get(key);

                if (CacheIsPersistent) {
                    item.LastPersistantSave = DateTime.Now;
                }

                Destination.AddOrUpdate(item);

                if (RemoveSourceObject) {
                    Source.Remove(key);
                }
            }
        }

    }
}
