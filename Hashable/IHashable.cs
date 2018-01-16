using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    public interface IHashable {
        /// <summary>
        /// A byte array of data to be hashed
        /// </summary>
        byte[] GetHashableData();

        /// <summary>
        /// Gets the hash of this object using a specified provider, and signs it with a certificate (if provided)
        /// </summary>
        void ComputeHash(Hash.HashProvider Provider, Cert SigningCert);

        bool VerifyHash();

        bool VerifySignature(Cert SigningPublicCert);

        /// <summary>
        /// Gets the hash of this object using the default provider
        /// </summary>
        Hash ComputedHash { get; }

    }
}
