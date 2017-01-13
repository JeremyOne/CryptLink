using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptLink
{
    public class Message : Hashable
    {
        public DateTime Sent { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string SenderAlias { get; set; }
        public string Pad { get; set; }
        public Hash.HashProvider HashProvider { get; set; }
        public int MaxLength { get; set; }
        
        public string GetJson() {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public bool MessageLengthValid() {
            var m = GetJson();

            if (m.Length > MaxLength) {
                //too long
                return false;
            } else if (m.Length == MaxLength) {
                //just right
                return true;
            } else {
                //too short
                int lengthDiff = MaxLength - m.Length;
                Pad = CryptLink.Crypto.GetRandomString(lengthDiff);
                return true;
            }
        }


        //Hashable abstract overrides
        public override byte[] HashableData() {
            UnicodeEncoding UE = new UnicodeEncoding();
            return UE.GetBytes(GetJson());
        }


    }
}
