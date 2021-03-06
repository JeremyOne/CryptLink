﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

    public interface IObjectCache : IDisposable {

        bool AcceptingObjects { get; set; }
        bool CacheIsPersistent { get; }
        bool CacheIsInitalized { get; }
        string ConnectionString { get; set; }

        long MinCollectionSize { get; set; }
        TimeSpan MinExpiration { get; set; }
        long MinObjectSize { get; set; }

        long MaxCollectionSize { get; set; }
        long MaxCollectionCount { get; set; }
        TimeSpan MaxExpiration { get; set; }
        long MaxObjectSize { get; set; }

        long CurrentCollectionCount { get; }
        long CurrentCollectionSize { get; }
        long CurrentReadCount { get; set; }
        long CurrentWriteCount { get; set; }

        DateTime StartedTime { get; }
        double CachePreference { get; set; }

        TimeSpan ManageEvery { get; set; }
        int ManageEveryIOCount { get; set; }
        long ManagedAtIOCount { get; set; }
        DateTime ManagedLastAt { get; set; }
        bool ManagementRunning { get; set; }

        /// <summary>
        /// Add or update an item in this cache
        /// </summary>
        bool AddOrUpdate<T>(ComparableBytesAbstract Key, T Value, TimeSpan ExpireSpan) where T : IHashable;
        bool AddOrUpdate(CacheItem Value);
        T Get<T>(ComparableBytesAbstract Key) where T : IHashable;
        T Get<T>(ComparableBytesAbstract Key, bool Recurse) where T : IHashable;
        CacheItem Get(ComparableBytesAbstract Key);
        CacheItem Get(ComparableBytesAbstract Key, bool Recurse);
        bool Expire(DateTime ExpiredAfter);
        bool Expire(DateTime ExpiredAfter, bool Recurse);
        bool Exists(ComparableBytesAbstract Key);
        bool Exists(ComparableBytesAbstract Key, bool Recurse);
        bool Remove(ComparableBytesAbstract Key);
        bool Remove(ComparableBytesAbstract Key, bool Recurse);

        double ComputeFitScore(CacheItem ForObject);

        double UseagePercent { get; }

        IObjectCache OverflowCache { get; set; }

        void Initialize();
        void Clear();

        void ManageCheck();
        void ManageNow();

        void CountRead();
        void CountWrite();
        double TotalAverageIOPS();

        void MigrateObjects(IObjectCache Source, IObjectCache Destination, ComparableBytesAbstract[] ObjectsToMove, bool RemoveSourceObject);

    }

}
