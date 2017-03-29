using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    public class BlockItem<V> : Hashable
        where V : Hashable {

        public HashableString Name { get; set; }
        public V Value { get; set; }
        public override Hash.HashProvider Provider { get; set; }

        public override bool HashIsImmutable {
            get {
                return false;
            }
        }

        public Hash Owner { get; set; }
        public DateTime Age { get; set; }
		public BlockItemStatus Status { get; set; }

		public int ConfirmedNodes { get; set; }
		public int UnconfirmedNodes { get; set; }

        public override byte[] HashableData() {
            return
	            Name.HashableData()
	             .Concat(Owner.Bytes)
                 .Concat(BitConverter.GetBytes((int)Provider))
	             .Concat(BitConverter.GetBytes(Age.Ticks))
	             .Concat(Value.HashableData()).ToArray();
        }
    }

	public enum BlockItemStatus{
		Pending,
		Comitted,
		Confirmed,
		Expired,
		Unknowm
	}

}
