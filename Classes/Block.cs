using System;
using System.Collections.Generic;

namespace CryptLink {
	public class Block<T> : Hashable
        where T : Hashable {

		public Hash ParentHash { get; set; }
		public Hash GeneratorAddress { get; set; }
		public DateTime GenerationDate { get; set; }
		public long BlockNumber { get; set; }
		public List<T> BlockItems { get; set; }
        public override Hash.HashProvider Provider { get; set; }

        public override bool HashIsImmutable {
            get {
                return false;
            }
        }

        public bool Verify(Block<T> Parent){
			throw new NotImplementedException();
		}

        public override byte[] HashableData() {
            throw new NotImplementedException();
        }
    }
}

