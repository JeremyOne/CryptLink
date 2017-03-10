using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink
{
    /// <summary>
    /// The abstract class that enables easy to use hashing and hashed based comparison
    /// </summary>
    public abstract class Hashable : IHashable {

        public abstract Hash.HashProvider Provider { get; set; }

        /// <summary>
        /// A byte array of data to be hashed
        /// </summary>
        public abstract byte[] HashableData();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool HashIsImmutable { get; }

        /// <summary>
        /// Get a Hash object for this class with a specified provider
        /// </summary>
        public Hash GetHash(Hash.HashProvider _Provider) {
            return Hash.Compute(HashableData(), _Provider);
        }

        /// <summary>
        /// Cache the hash
        /// </summary>
        private Hash _Hash = default(Hash);

        /// <summary>
        /// Cache the bytes
        /// </summary>
        private byte[] _HashBytes = null;

        /// <summary>
        /// Get a Hash object for this class with the default provider
        /// </summary>       
        public Hash Hash {
            get {
                if (HashIsImmutable) {
                    if (_Hash?.Bytes == null) {
                        _Hash = Hash.Compute(HashableData(), Provider);
                        _HashBytes = _Hash.Bytes;
                    }

                    return _Hash;
                } else {
                    return Hash.Compute(HashableData(), Provider);
                }
            }
        }

        [LiteDB.BsonId, LiteDB.BsonIndex]
        public byte[] HashBytes {
            get {
                if (_HashBytes != null) {
                    return _HashBytes;
                } else {
                    return Hash.Bytes;
                }
            }
        }

        /// <summary>
        /// Returns if the specified object is a subclass of Hashable
        /// </summary>
        public static bool IsHashable(object obj) {
            return obj.GetType().IsSubclassOf(typeof(Hashable));
        }

    }
}
