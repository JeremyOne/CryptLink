using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

    /// <summary>
    /// A string that can easily be converted to a Hash
    /// </summary>
    public class HashableString : Hashable {


        public HashableString(string _Value) {
            Value = _Value;
        }

        /// <summary>
        /// Create a new hashable string 
        /// </summary>
        /// <param name="_Value">The string to hash</param>
        /// <param name="Noramilize">If true, trims whitespace and set to lowercase invariant</param>
        public HashableString(string _Value, bool Noramilize) {
            if (Noramilize) {
                Value = _Value.ToLowerInvariant().Trim();
            } else {
                Value = _Value;
            }
        }

        public string Value { get; private set; }

        public override byte[] HashableData() {
            if (string.IsNullOrEmpty(Value) == false) {
                return Encoding.ASCII.GetBytes(Value);
            } else {
                throw new Exception("Value was null");
            }           
        }
    }
}
