using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

    /// <summary>
    /// User - A Users of the network
    /// </summary>
    class User : Hashable {

        public byte[] PublicKey { get; set; }
        public string Name { get; set; }
        public DateTime LastActivity { get; set; }
        public override Hash.HashProvider Provider { get; set; }

        public override bool HashIsImmutable {
            get {
                return true;
            }
        }

        public override byte[] HashableData() {
            return PublicKey;
        }

    }
}
