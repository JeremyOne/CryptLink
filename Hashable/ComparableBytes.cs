using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

    /// <summary>
    /// A minimal class for implementing ComparableBytesAbstract
    /// </summary>
    class ComparableBytes : ComparableBytesAbstract {
        public override byte[] Bytes { get; set; }

        public ComparableBytes() { }
        public ComparableBytes(byte[] FromBytes) {
            Bytes = FromBytes;
        }
    }
}
