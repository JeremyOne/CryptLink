using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using ServiceStack;
using NLog;
using System.Reflection;


namespace CryptLink {
	public interface IServer : IDisposable {
		Hash.HashProvider Provider { get; }
		X509Certificate2 ServerCert { get; set; }
		IObjectCache ObjectCache { get; set; }
		string ServiceAddress { get; set; }
		long MessageCount { get; set; }
		Logger logger { get; set; }
		Peer ThisPeerInfo { get; set; }
		ConsistentHash<Peer> KnownPeers { get; set; }

		void Init();

		new void Dispose();
		T Get<T>(ComparableBytesAbstract Key) where T : Hashable;

		Peer GetPeer(ComparableBytesAbstract Key);
	}
}

