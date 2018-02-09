using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static CryptLink.Hash;

namespace CryptLink {

    /// <summary>
    /// A immutable string that can easily be hashed
    /// </summary>
    public class HashableString : Hashable {

        public HashableString() { }
        
        public HashableString(string _Value, HashProvider _Provider, Cert _SigningCert = null) {
            Value = _Value;
            ComputeHash(_Provider, _SigningCert);
        }
        
        /// <summary>
        /// Create a new Hashable string 
        /// </summary>
        /// <param name="_Value">The string to hash</param>
        /// <param name="Noramilize">If true, trims whitespace and set to lowercase invariant</param>
        public HashableString(string _Value, bool Noramilize, HashProvider _Provider, Cert _SigningCert = null) {
            if (Noramilize) {
                Value = _Value.ToLowerInvariant().Trim();
            } else {
                Value = _Value;
            }

            ComputeHash(_Provider, _SigningCert);
        }

        public string Value { get; }

        public override byte[] GetHashableData() {
            if (string.IsNullOrEmpty(Value) == false) {
                return Encoding.ASCII.GetBytes(Value);
            } else {
                throw new Exception("Value was null");
            }
        }

    }
}
