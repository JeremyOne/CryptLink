using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

    public interface IObjectCache : IDisposable {

        bool AcceptingObjects { get; set; }
        bool CacheIsPersistent { get; }
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

        bool AddOrUpdate<T>(CByte Key, T Value, TimeSpan ExpireSpan) where T : Hashable;
        bool AddOrUpdate(CacheItem Value);
        T Get<T>(CByte Key) where T : Hashable;
        CacheItem Get(CByte Key);
        bool Expire(DateTime ExpiredAfter);
        bool Exists(CByte Key);
        bool Remove(CByte Key);

        double ComputeFitScore(CacheItem ForObject);

        double UseagePercent { get; }

        IObjectCache OverflowCache { get; set; }

        void Initialize();

        void ManageCheck();
        void ManageNow();

        void CountRead();
        void CountWrite();
        double TotalAverageIOPS();

        void MigrateObjects(IObjectCache Source, IObjectCache Destination, CByte[] ObjectsToMove, bool RemoveSourceObject);

    }

}
