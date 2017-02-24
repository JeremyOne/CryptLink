using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink
{
    public class Hash : CByte {
        public HashProvider Provider { get; private set; }

        private readonly byte[] _bytes;

        public override byte[] Bytes {
            get {
                return _bytes;
            }
        }

        static HashAlgorithm[] hashAlgorithms = new HashAlgorithm[Enum.GetNames(typeof(HashProvider)).Length];

        public Hash() { }

        /// <summary>
        /// Creates a immutable hash object with the specified bytes. NOTE: this does not COMPUTE a hash, use Hash.Compute
        /// </summary>
        /// <param name="HashedBytes">The bytes to copy into this hash</param>
        /// <param name="_Provider"></param>
        private Hash(byte[] HashedBytes, HashProvider _Provider) {

            if (HashedBytes.Length == GetProviderByteLength(_Provider)) {
                _bytes = HashedBytes;
                Provider = _Provider;
            } else {
                throw new ArgumentException("The provided bytes are not the expected length, should be: "
                    + GetProviderByteLength(_Provider) +
                    " but was actually: " + HashedBytes.Length);
            }            
        }

        /// <summary>
        /// Copies the bytes from a b64 string to a new Hash (does not compute a hash)
        /// </summary>
        public static Hash FromB64(string Base64String, HashProvider _Provider) {
            var bytes = Base64.DecodeBytes(Base64String);
            return FromComputedBytes(bytes, _Provider);
        }

        /// <summary>
        /// Copies a pre-computed hash bytes in a Hash object
        /// </summary>
        /// <param name="PreComputedHashBytes"></param>
        /// <param name="_Provider"></param>
        /// <returns></returns>
        public static Hash FromComputedBytes(byte[] PreComputedHashBytes, HashProvider _Provider) {
            if (PreComputedHashBytes.Length == GetProviderByteLength(_Provider)) {
                return new Hash(PreComputedHashBytes, _Provider);
            } else {
                throw new ArgumentException("Provided bytes were not the expected length for this hash type");
            }
        }

        /// <summary>
        /// Gets the number of bytes for the current hash provider
        /// </summary>
        public int HashByteLength(bool ZeroIndexed) {
            return GetProviderByteLength(Provider);
        }

        public static int GetProviderByteLength(HashProvider ForProvider) {
            switch (ForProvider) {
                case Hash.HashProvider.MD5:
                    return 16;
                case Hash.HashProvider.SHA1:
                    return 20;
                case Hash.HashProvider.SHA256:
                    return 32;
                case Hash.HashProvider.SHA384:
                    return 48;
                case Hash.HashProvider.SHA512:
                    return 64;
                default:
                    throw new NotImplementedException("Hash provider '" + ForProvider.ToString() + "' not implemented in GetHashByteLength");
            }
        }

        public static string GetOIDForProvider(HashProvider Provider) {
            return CryptoConfig.MapNameToOID(Provider.ToString());
        }

        public static Hash Compute(string FromString, HashProvider Provider) {
            UnicodeEncoding UE = new UnicodeEncoding();
            return Compute(UE.GetBytes((string)FromString), Provider);
        }

        /// <summary>
        /// Creates/computes a hash from the public key of a X509Cert
        /// </summary>
        /// <param name="FromX509">X509 cert with public key</param>
        /// <param name="HashProvider">The provider that calculated the hash</param>
        public static Hash Compute(X509Certificate2 FromX509, HashProvider HashProvider) {
            return Compute(FromX509.GetPublicKey(), HashProvider);
        }

        /// <summary>
        /// Computes the hash from a byte[] and sets the HashProvider
        /// </summary>
        /// <param name="From">Bytes to compute the hash from</param>
        /// <param name="HashProvider">The crypto provider to compute the hash with</param>
        public static Hash Compute(byte[] HashFromBytes, HashProvider HashProvider) {
            HashAlgorithm hashAlgo = GetHashAlgorithm(HashProvider);
            return new Hash(hashAlgo.ComputeHash(HashFromBytes), HashProvider);
        }

        /// <summary>
        /// Gets a HashAlgorithm from a HashProvider using a no-search static array
        /// </summary>
        private static HashAlgorithm GetHashAlgorithm(HashProvider Provider) {
            
            if (hashAlgorithms[(int)Provider] == null) {
                var h = HashAlgorithm.Create(Provider.ToString());
                hashAlgorithms[(int)Provider] = h;            
            }

            return hashAlgorithms[(int)Provider];
        }

        /// <summary>
        /// Generate a new hash, by re-hashing the current bytes
        /// Useful for making a new re-producible hash(s) for distributing in a ConsistentHash table
        /// </summary>
        public Hash Rehash() {
            return Hash.Compute(Bytes, Provider);
        }
        
        public enum HashProvider {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

    }


}
