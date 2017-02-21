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
    public abstract class Hashable
    {
        /// <summary>
        /// A byte array of data to be hashed
        /// </summary>
        public abstract byte[] HashableData();
        
        /// <summary>
        /// Get a Hash object for this class
        /// </summary>
        public Hash GetHash(Hash.HashProvider Provider) {
            return new Hash(HashableData(), Provider);
        }
        
        /// <summary>
        /// Gets a Hash object from a object that inherits from Hashable
        /// </summary>
        /// <param name="FromObject">Object to hash</param>
        /// <param name="Provider">The hash provider to hash with</param>
        public static Hash GetHash<H>(H FromObject, Hash.HashProvider Provider) where H : Hashable {
            return FromObject.GetHash(Provider);
        }

        /// <summary>
        /// Returns if the specified object is a subclass of Hashable
        /// </summary>
        public static bool IsHashable(object obj) {
            return obj.GetType().IsSubclassOf(typeof(Hashable));
        }
    }
}
