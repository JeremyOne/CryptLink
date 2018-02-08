using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

    public interface IPeerTransport : IDisposable {
        string Address { get; set; }

        DateTime LastSeen { get; }
        DateTime FirstSeen { get; }
        Dictionary<DateTime, long> TrafficHistory { get; }

        int Weight { get; set; }

        TimeSpan TransportTimeOut { get; set; }
        int ConnectRetryMax { get; set; }
        int ConnectRetriesCurrent { get; set; }

        bool TryConnect();
        bool Connected();

        T TryGet<T>(ComparableBytesAbstract Key) where T : Hashable;
        CacheItem TryGet(ComparableBytesAbstract Key);

        bool TryPut<T>(T Item) where T : Hashable;
        bool TryPut(CacheItem Item);

        TransportDirection PeerDirection { get; set; }

        bool Ready { get; }
    }

    public enum TransportDirection {
        BiDirectional,
        ReceveOnly,
        RequestOnly
    }
}
