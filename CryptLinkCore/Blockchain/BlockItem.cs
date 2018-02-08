using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace CryptLink {
    public class BlockItem<V> : Hashable
        where V : Hashable {

        public HashableString Name { get; set; }
        public V Value { get; set; }
        
        public Hash Owner { get; set; }
        public DateTime Age { get; set; }
		public BlockItemStatus Status { get; set; }

		public int ConfirmedNodes { get; set; }
		public int UnconfirmedNodes { get; set; }

        public override byte[] GetHashableData() {
            return
	            Name.GetHashableData()
	             .Concat(Owner.Bytes)
	             .Concat(BitConverter.GetBytes(Age.Ticks))
	             .Concat(Value.GetHashableData()).ToArray();
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
