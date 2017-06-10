using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CryptLink {
	public class Swarm {
        public int RootCertMinLength { get; set; }
        public int ServerCertMinLength { get; set; }
        public int UserCertMinLength { get; set; }

        public string SwarmName { get; set; }

        public Hash.HashProvider Provider { get; set; }

        [JsonConverter(typeof(CertificateSerializer))]
        public X509Certificate2 PrivateKey { get; set; }

        public int RootPeerWeight { get; set; } = 150;
        public int KnownPeerWeight { get; set; } = 100;

        public List<Peer> RootPeers { get; set; }
        public List<Peer> KnownPeers { get; set; }
        public Uri PublicAddress { get; set; }
		public JoinAccessibility Accessibility { get; set; }

		public bool PaddingEnforced { get; set; }

		public TimeSpan MessageMinStorage { get; set; }
		public long MessageMinLength { get; set; }
		public long MessageMaxLength { get; set; }

		public TimeSpan ItemMinStorage { get; set; }
		public long ItemMinLength { get; set; }
		public long ItemMaxLength { get; set; }

		public TimeSpan BlobMinStorage { get; set; }
		public long BlobMinLength { get; set; }
		public long BlobMaxLength { get; set; }

		//public BlockChain<Hash> BlacklistedPeers { get; set; }

		public Swarm() {

		}

		public enum JoinAccessibility {
			NoRestrictions,
			RootTimeRestricted,
			PresharedKeyRequired,
			VoucherRequired,
			ValidCertRequired
		}
	}
}

