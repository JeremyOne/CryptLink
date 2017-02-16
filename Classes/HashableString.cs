using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

    /// <summary>
    /// A simple Hashable object used mainly for testing
    /// </summary>
    public class HashableString : Hashable {

        public HashableString(string _Value) {
            Value = _Value;
        }

        string Value { get; set; }

        public override byte[] HashableData() {
            if (string.IsNullOrEmpty(Value) == false) {
                return Encoding.ASCII.GetBytes(Value);
            } else {
                throw new Exception("Value was null");
            }           
        }
    }
}
