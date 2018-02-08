using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace CryptLink {

    /// <summary>
    /// User - A Users of the network
    /// </summary>
    class User : Hashable {

        public X509Certificate2 Cert { get; set; }
        public string Name { get; set; }
        public DateTime LastActivity { get; set; }
        
        public override byte[] GetHashableData() {
            return Cert.PublicKey.EncodedKeyValue.RawData;
        }

    }
}
