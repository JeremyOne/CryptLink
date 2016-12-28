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
    public class Hash : IComparable
    {
        public HashProviders Provider { get; set; }
        public byte[] Hashed { get; set; }
        public bool Valid { get; set; }

        public Hash(Message From) {
            UnicodeEncoding UE = new UnicodeEncoding();
            ComputeHash(UE.GetBytes(From.GetJson()), From.HashProvider);
            Valid = true;
        }

        public Hash(string From, HashProviders HashProvider) {
            UnicodeEncoding UE = new UnicodeEncoding();
            ComputeHash(UE.GetBytes((string)From), HashProvider);

            if (string.IsNullOrWhiteSpace(From)) {
                Valid = false;
            } else {
                Valid = true;
            }
        }

        public Hash() { }

        public Hash(byte[] From, HashProviders HashProvider) {
            ComputeHash(From, HashProvider);
        }

        public Hash(PeerDetails From, HashProviders HashProvider) {

            ComputeHash(From.Cert.GetCertHash(), HashProvider);
        }

        /// <summary>
        /// Create a hash object from a b64 encoded byte array
        /// </summary>
        /// <param name="FromBase64Encode">B64 encoded bytes, using Crypto.DecodeBytes() b64 format</param>
        /// <param name="HashProvider">The provider that calculated the hash</param>
        public static Hash FromB64(string FromBase64Encode, HashProviders HashProvider) {
            var h = new Hash();
            h.Hashed = Crypto.DecodeBytes(FromBase64Encode);
            h.Provider = HashProvider;
            return h;
        }


        /// <summary>
        /// Create a hash object from a X509 Cert
        /// </summary>
        /// <param name="FromX509">X509 cert with public key</param>
        /// <param name="HashProvider">The provider that calculated the hash</param>
        public static Hash FromX509(X509Certificate2 FromX509, HashProviders HashProvider) {
            var h = new Hash();
            h.ComputeHash(FromX509.GetPublicKey(), HashProvider);
            return h;
        }

        public static Hash FromHttpClientCert(System.Web.HttpClientCertificate FromCert, HashProviders HashProvider) {
            var h = new Hash();
            h.ComputeHash(FromCert.PublicKey, HashProvider);
            return h;
        }

        /// <summary>
        /// Create a hash object from a pre-computed hash
        /// </summary>
        /// <param name="HashBytes">The value of the hash</param>
        /// <param name="HashProvider">The provider that calculated the hash, if null assume SHA256</param>
        public static Hash FromBinaryHash(byte[] HashBytes, HashProviders? HashProvider) {
            var h = new Hash();
            h.Hashed = HashBytes;

            if (HashProvider.HasValue) {
                h.Provider = HashProvider.Value;
            } else {
                h.Provider = HashProviders.SHA256;
            }
            
            return h;
        }


        /// <summary>
        /// Computes the hash from a bute[] and sets the HashProvider
        /// </summary>
        /// <param name="From">Bytes to compute the hash from</param>
        /// <param name="HashProvider">The crypto provider to compute the hash with</param>
        private void ComputeHash(byte[] From, HashProviders HashProvider){
            int truncateBytes = 0;
            Provider = HashProvider;

            //set the provider for truncated types
            if (HashProvider == HashProviders.SHA128) {
                HashProvider = HashProviders.SHA256;
                truncateBytes = 128;
            } else if (HashProvider == HashProviders.SHA64) {
                HashProvider = HashProviders.SHA256;
                truncateBytes = 64;
            }

            byte[] hashValue;

            //actual computation
            if (HashProvider == HashProviders.Murmer32) {
                uint h = MurmurHash2.Hash(From);
                hashValue = BitConverter.GetBytes(h);
            } else {
                HashAlgorithm hash = HashAlgorithm.Create(HashProvider.ToString());
                hashValue = hash.ComputeHash(From);
            }
            
            //truncate if needed
            if(truncateBytes > 0) {
                Array.Copy(hashValue, Hashed, truncateBytes);
            } else {
                Hashed = hashValue;
            }

        }

        /// <summary>
        /// Generate a new hash, based on this with an added seed. 
        /// Used for making a new re-producible hash(s) for distributing in a ConsistentHash table
        /// </summary>
        /// <param name="Seed">Added to hash before re-hashing, a zero seed does not affect the hash</param>
        /// <returns>A new hash</returns>
        public Hash Rehash(int Seed) {
            if (Seed == 0) {
                return this;
            }

            byte[] newBytes = Combine(Hashed, BitConverter.GetBytes(Seed));
            return new Hash(newBytes, Provider);
        }

        /// <summary>
        /// Combines byte arrays
        /// </summary>
        private byte[] Combine(params byte[][] arrays) {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays) {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        /// <summary>
        /// Returns a basic B64 representation of the hash
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return System.Convert.ToBase64String(Hashed);
        }

        public static int Compare(Hash Left, Hash Right) {
            return Compare(Left.Hashed, Right.Hashed);
        }

        public static int Compare(object Left, object Right) {
            byte[] bLeft = new byte[] { 0x00 };
            byte[] bRight;

            if (Left is Hash) {
                var h = (Hash)Left;
                bLeft = h.Hashed;
            } else if (Left is byte[]) {
                bLeft = (byte[])Left;
            } else {
                throw new NotImplementedException("Can't compare CryptLink.Hash to a type of '" +
                    Left.GetType().ToString() + "'. Valid types are CryptLink.Hash and byte[]");
            }

            if (Right is Hash) {
                var h = (Hash)Right;
                bRight = h.Hashed;
            } else if (Right is byte[]) {
                bRight = (byte[])Right;
            } else {
                bRight = new byte[] { 0x00 };
                throw new NotImplementedException("Can't compare CryptLink.Hash to a type of '" +
                    Right.GetType().ToString() + "'. Valid types are CryptLink.Hash and byte[]");
            }

            return Compare(bLeft, bRight);
        }

        /// <summary>
        /// Implement IComparable so that hashes can be compared and ordered in the ConsistantHash table
        /// </summary>
        /// <returns>1, 0 or -1</returns>
        public static int Compare(byte[] Left, byte[] Right) {
            int compareLength;

            //the hashes could be a different length, that's okay, we'll only compare the shared bits
            compareLength = Math.Min(Left.Length, Right.Length);

            //Compare by finding the first different bit, this should allow for very efficient comparisons
            //if there are no different bytes, continue
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
            //this should prevent lower bit (less secure) collisions being equal to longer hashes
            if (Left.Length > Right.Length) {
                return 1;
            } else if (Left.Length < Right.Length) {
                return -1;
            }
            
            //if they are the same length, they are equal
            return 0;
        }

        //Below here are references to Compare() required for c# comparisons and operators ------
        //note that equality is compared on the data, not on reference

        public int CompareTo(object obj) {
            return Compare(this, obj);
        }

        public int CompareTo(Hash obj) {
            return Compare(this.Hashed, obj.Hashed);
        }

        public override bool Equals(object obj) {
            return CompareTo(obj) == 0;
        }

        public bool Equals(Hash obj) {
            return CompareTo(obj) == 0;
        }

        /// <summary>
        /// Depreciated and used for compatibility only, do not use!
        /// Needed for OverrideGetHashCodeOnOverridingEquals
        /// </summary>
        public override int GetHashCode() {
            return BitConverter.ToInt32(Hashed, 0);
        }


        //Below here are the operators, so we can do something like:
        //(Hash1 > Hash2) and get a bool result

        public static bool operator ==(Hash Left, object Right) {
            return Compare(Left, Right) == 0;
        }

        public static bool operator ==(Hash Left, Hash Right) {
            return Compare(Left, Right) == 0;
        }

        public static bool operator !=(Hash Left, object Right) {
            return Compare(Left, Right) != 0;
        }

        public static bool operator !=(Hash Left, Hash Right) {
            return Compare(Left, Right) != 0;
        }

        public static bool operator >(Hash Left, object Right) {
            return Compare(Left, Right) > 0;
        }

        public static bool operator >(Hash Left, Hash Right) {
            return Compare(Left, Right) > 0;
        }

        public static bool operator <(Hash Left, object Right) {
            return Compare(Left, Right) < 0;
        }

        public static bool operator <(Hash Left, Hash Right) {
            return Compare(Left, Right) < 0;
        }

        public static bool operator >=(Hash Left, object Right) {
            return Compare(Left, Right) >= 0;
        }

        public static bool operator >=(Hash Left, Hash Right) {
            return Compare(Left, Right) >= 0;
        }

        public static bool operator <=(Hash Left, object Right) {
            return Compare(Left, Right) <= 0;
        }

        public static bool operator <=(Hash Left, Hash Right) {
            return Compare(Left, Right) <= 0;
        }

        public enum HashProviders {
            Murmer32,
            SHA64,
            SHA128,
            SHA256,
            SHA512
        }
    }


}
