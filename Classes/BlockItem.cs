using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    public class BlockItem<T> : Hashable
        where T : Hashable {

        public string Name { get; set; }
        public Hash Owner { get; set; }
        public DateTime Age { get; set; }
        public T Value { get; set; }
		public BlockItemStatus Status { get; set; }
		public int ConfirmedNodes { get; set; }
		public int UnconfirmedNodes { get; set; }

        public override byte[] HashableData() {
            return
	            Encoding.ASCII.GetBytes(Name)
	             .Concat(Owner.HashBytes)
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
