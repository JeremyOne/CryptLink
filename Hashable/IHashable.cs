using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    public interface IHashable {
        /// <summary>
        /// A byte array of data to be hashed
        /// </summary>
        byte[] HashableData();

        /// <summary>
        /// The default HashProvider for this object
        /// </summary>
        Hash.HashProvider Provider { get; set; }

        /// <summary>
        /// Gets the hash of this object using a specified provider
        /// </summary>
        Hash GetHash(Hash.HashProvider Provider);

        /// <summary>
        /// Gets the hash of this object using the default provider
        /// </summary>
        Hash Hash { get; }

    }
}
