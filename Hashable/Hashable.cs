using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink
{
    /// <summary>
    /// The abstract class that enables easy to use hashing and hashed based comparison
    /// </summary>
    public abstract class Hashable : IHashable {

        /// <summary>
        /// A byte array of data to be hashed
        /// </summary>
        public abstract byte[] GetHashableData();
        

        [LiteDB.BsonId]
        public byte[] ComputedHashBytes {
            get {
                if (ComputedHash != null) {
                    return ComputedHash.Bytes;
                } else {
                    throw new NullReferenceException("No hash exists");
                }
            }
        }

        /// <summary>
        /// The computed hash for this object, will be null until ComputeHash() is called
        /// </summary>
        public Hash ComputedHash { get; private set; }

        /// <summary>
        /// Compute a hash for this object and optionally signs it
        /// </summary>
        /// <param name="Provider">The hash provider to use</param>
        /// <param name="SigningCert">If provided the cert to sign the hash with</param>
        public void ComputeHash(Hash.HashProvider Provider, Cert SigningCert = null) {
            ComputedHash = Hash.Compute(GetHashableData(), Provider, SigningCert);
        }
        
        /// <summary>
        /// Verifies the current hash
        /// </summary>
        /// <returns>Returns TRUE if hash verifies correctly</returns>
        public bool VerifyHash() {
            if (ComputedHash == null) {
                throw new NullReferenceException("This hashable object does not have a computed hash, call ComputeHash() first.");
            } else {
                var newHash = Hash.Compute(GetHashableData(), ComputedHash.Provider);
                return ComputedHash == newHash;
            }
        }

        /// <summary>
        /// Verifies the hash and signature of an object
        /// </summary>
        /// <param name="SigningPublicCert"></param>
        /// <returns>Returns TRUE if the hash and signature verify correctly</returns>
        public bool VerifySignature(Cert SigningPublicCert) {
            if (VerifyHash() == false) {
                return false;
            }

            return ComputedHash.Verify(GetHashableData(), SigningPublicCert);
        }
    }
}
