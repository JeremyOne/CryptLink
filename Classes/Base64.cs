using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    class Base64 {

        /// <summary>
        /// Encodes bytes into url safe base64 (uses _-. instead of /+=)
        /// </summary>
        public static string EncodeBytes(byte[] Bytes) {
            if (Bytes == null) {
                return null;
            }
            var ks = System.Convert.ToBase64String(Bytes, 0, Bytes.Length);
            return ks.Replace('/', '_').Replace('+', '-').Replace('=', '.');
        }

        public static Nullable<Int64> DecodeInt64(string Input) {
            var decoded = DecodeBytes(Input);

            if (decoded != null) {
                return BitConverter.ToInt64(decoded, 0);
            } else {
                return null;
            }
        }

        /// <summary>
        /// Decodes bytes from base64 (Replaces _-. with /+=)
        /// </summary>
        public static byte[] DecodeBytes(string BytesB64Encoded) {
            if (string.IsNullOrWhiteSpace(BytesB64Encoded)) {
                return null;
            }

            BytesB64Encoded = BytesB64Encoded.Replace('_', '/').Replace('-', '+').Replace('.', '=');

            try {
                Byte[] b = Convert.FromBase64String(BytesB64Encoded);
                return b;
            } catch (FormatException e) {
                return null;
            }
        }


    }
}
