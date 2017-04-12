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
		Hash.HashProvider Provider { get; set; }
		X509Certificate2 ServerCert { get; set; }
		IObjectCache ObjectCache { get; set; }
		string ServiceAddress { get; set; }
		long MessageCount { get; set; }
		Logger logger { get; set; }
		Peer ThisPeerInfo { get; set; }

		ServiceStackHost ServiceHost { get; set; }

		ConsistentHash<Peer> KnownPeers { get; set; }

		void StartServices();

		new void Dispose();
		T Get<T>(CByte Key) where T : Hashable;

		Peer GetPeer(CByte Key);
	}
}

