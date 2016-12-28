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

    }
}
