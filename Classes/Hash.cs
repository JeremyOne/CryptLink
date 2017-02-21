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
    public class Hash : IComparable {
        public HashProvider Provider { get; private set; }
        public byte[] HashBytes { get; private set; }
        public bool Valid { get; private set; }

        static HashAlgorithm[] hashAlgorithms = new HashAlgorithm[Enum.GetNames(typeof(HashProvider)).Length];

        /// <summary>
        /// Creates a new hash instance from a string that will be hashed
        /// </summary>
        /// <param name="StringToHash">String used to compute the hash</param>
        /// <param name="HashProvider"></param>
        public Hash(string StringToHash, HashProvider HashProvider) {
            UnicodeEncoding UE = new UnicodeEncoding();
            ComputeHash(UE.GetBytes((string)StringToHash), HashProvider);

            if (string.IsNullOrWhiteSpace(StringToHash)) {
                Valid = false;
            } else {
                Valid = true;
            }
        }

        /// <summary>
        /// Creates a new hash instance from an array of bytes that will be hashed
        /// </summary>
        /// <param name="BytesToHash">Bytes used to compute the hash</param>
        public Hash(byte[] BytesToHash, HashProvider HashProvider) {

            if (BytesToHash == null) {
                Valid = false;
            } else {
                Valid = true;
            }

            ComputeHash(BytesToHash, HashProvider);

        }

        public Hash() { }

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

        /// <summary>
        /// Create a hash object from a b64 encoded byte array
        /// </summary>
        /// <param name="FromBase64">B64 encoded bytes</param>
        /// <param name="HashProvider">The provider that calculated the hash</param>
        public static Hash FromB64(string FromBase64, HashProvider HashProvider) {
            var h = new Hash();

            h.HashBytes = Base64.DecodeBytes(FromBase64);
            h.Provider = HashProvider;

            return h;
        }
        
        /// <summary>
        /// Create a hash object from a X509 Cert
        /// </summary>
        /// <param name="FromX509">X509 cert with public key</param>
        /// <param name="HashProvider">The provider that calculated the hash</param>
        public static Hash FromX509(X509Certificate2 FromX509, HashProvider HashProvider) {
            var h = new Hash();
            h.ComputeHash(FromX509.GetPublicKey(), HashProvider);
            return h;
        }

        public static Hash FromHttpClientCert(System.Web.HttpClientCertificate FromCert, HashProvider HashProvider) {
            var h = new Hash();
            h.ComputeHash(FromCert.PublicKey, HashProvider);
            return h;
        }

        /// <summary>
        /// Create a hash object from a pre-computed hash
        /// </summary>
        /// <param name="HashBytes">The value of the hash</param>
        /// <param name="HashProvider">The provider that calculated the hash, if null assume SHA256</param>
        public static Hash FromBinaryHash(byte[] HashBytes, HashProvider HashProvider) {
            var h = new Hash();
            h.HashBytes = HashBytes;
            h.Provider = HashProvider;
            return h;
        }

        /// <summary>
        /// Computes the hash from a bute[] and sets the HashProvider
        /// </summary>
        /// <param name="From">Bytes to compute the hash from</param>
        /// <param name="HashProvider">The crypto provider to compute the hash with</param>
        private void ComputeHash(byte[] From, HashProvider HashProvider) {
            int truncateBytes = 0;
            Provider = HashProvider;

            byte[] hashValue;

            //hash
            HashAlgorithm hash = GetHashAlgorithm(HashProvider);
            hashValue = hash.ComputeHash(From);

            //truncate if needed
            if (truncateBytes > 0) {
                HashBytes = new byte[truncateBytes];
                Array.Copy(hashValue, HashBytes, truncateBytes);
            } else {
                HashBytes = hashValue;
            }

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
        /// Used for making a new re-producible hash(s) for distributing in a ConsistentHash table
        /// </summary>
        public Hash Rehash() {

            var h = new Hash() {
                Provider = Provider,
                Valid = true
            };

            h.ComputeHash(HashBytes, Provider);

            return h;
        }

        /// <summary>
        /// Returns a basic B64 representation of the hash
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Convert.ToBase64String(HashBytes);
        }

        public static int Compare(Hash Left, Hash Right) {
            if (ReferenceEquals(Right, null)) {
                return -1;
            } else if (ReferenceEquals(Left, null)) {
                return 1;
            }

            return Compare(Left.HashBytes, Right.HashBytes);
        }

        public static int Compare(byte[] Left, Hash Right) {
            return Compare(Left, Right.HashBytes);
        }

        public static int Compare(Hash Left, byte[] Right) {
            return Compare(Left.HashBytes, Right);
        }

        /// <summary>
        /// Compares two byte arrays of any length
        /// </summary>
        /// <returns>1, 0 or -1</returns>
        public static int Compare(byte[] Left, byte[] Right) {
            int compareLength;

            //the hashes could be a different length, that's okay, we'll only compare the shared bits
            compareLength = Math.Min(Left.Length, Right.Length);

            if (BitConverter.IsLittleEndian) {
                //read in Little Endian order
                for (int i = 0; i < compareLength; i++) {
                    if (Left[i] > Right[i]) {
                        return 1;
                    } else if (Left[i] > Right[i]) {
                        return -1;
                    }
                }
            } else {
                //read in Big Endian order
                for (int i = compareLength; i > 0; i--) {
                    if (Left[i] > Right[i]) {
                        return 1;
                    } else if (Left[i] > Right[i]) {
                        return -1;
                    }
                }
            }

            //All the shared bits are the same, so return the longer hash
            if (Left.Length > Right.Length) {
                return 1;
            } else if (Left.Length < Right.Length) {
                return -1;
            }

            //Same bits and length, arrays are equal
            return 0;
        }

        #region // ICompariable methods

        public int CompareTo(byte[] Bytes) {
            return Compare(this, Bytes);
        }

        public int CompareTo(Hash HashObject) {
            return Compare(this.HashBytes, HashObject.HashBytes);
        }

        public int CompareTo(object obj) {
            if (obj == null) {
                return 1;
            } else if (obj is Hash) {
                return CompareTo((Hash)obj);
            } else if (obj is byte[]) {
                return CompareTo((byte[])obj);
            }

            return 1;
        }

        public static explicit operator byte[] (Hash b) {
            return b.HashBytes;
        }

        /// <summary>
        /// Compare to object, will always return false for any object that is not Hash or byte[] 
        /// (Required for IComparable)
        /// </summary>
        public override bool Equals(object obj) {
            if (obj is Hash) {
                return CompareTo((Hash)obj) == 0;
            } else if (obj is byte[]) {
                return CompareTo((byte[])obj) == 0;
            }

            return false;
        }

        public bool Equals(Hash HashObject) {
            return CompareTo(HashObject) == 0;
        }

        #endregion 

        /// <summary>
        /// Depreciated and implemented for compatibility only, this hash is expressed as a Int32 (only 4 bytes).
        /// (Needed for OverrideGetHashCodeOnOverridingEquals)
        /// </summary>
        public override int GetHashCode() {
            return BitConverter.ToInt32(HashBytes, 0);
        }
        
        #region // All common operators needed for boolean operations

        /*  ==  Equals */
        public static bool operator ==(Hash Left, byte[] Right) {
            return Compare(Left, Right) == 0;
        }

        public static bool operator ==(byte[] Left, Hash Right) {
            return Compare(Left, Right) == 0;
        }

        public static bool operator ==(Hash Left, Hash Right) {
            return Compare(Left, Right) == 0;
        }
        
        /*  !=  Not equals */
        public static bool operator !=(Hash Left, byte[] Right) {
            return Compare(Left, Right) != 0;
        }

        public static bool operator !=(byte[] Left, Hash Right) {
            return Compare(Left, Right) != 0;
        }

        public static bool operator !=(Hash Left, Hash Right) {
            return Compare(Left, Right) != 0;
        }

        /*  >  More than */
        public static bool operator >(Hash Left, byte[] Right) {
            return Compare(Left, Right) > 0;
        }

        public static bool operator >(byte[] Left, Hash Right) {
            return Compare(Left, Right) > 0;
        }

        public static bool operator >(Hash Left, Hash Right) {
            return Compare(Left, Right) > 0;
        }

        /*  <  Less than */
        public static bool operator <(Hash Left, byte[] Right) {
            return Compare(Left, Right) < 0;
        }

        public static bool operator <(Hash Left, Hash Right) {
            return Compare(Left, Right) < 0;
        }

        public static bool operator <(byte[] Left, Hash Right) {
            return Compare(Left, Right) < 0;
        }

        /*  =>  More than or equals */
        public static bool operator >=(Hash Left, byte[] Right) {
            return Compare(Left, Right) >= 0;
        }

        public static bool operator >=(Hash Left, Hash Right) {
            return Compare(Left, Right) >= 0;
        }

        public static bool operator >=(byte[] Left, Hash Right) {
            return Compare(Left, Right) >= 0;
        }

        /*  <=  Less than or equals */
        public static bool operator <=(Hash Left, byte[] Right) {
            return Compare(Left, Right) <= 0;
        }

        public static bool operator <=(Hash Left, Hash Right) {
            return Compare(Left, Right) <= 0;
        }

        public static bool operator <=(byte[] Left, Hash Right) {
            return Compare(Left, Right) <= 0;
        }

        #endregion

        public enum HashProvider {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

    }


}
