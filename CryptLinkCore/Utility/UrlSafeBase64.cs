using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

    /// <summary>
    /// A very basic implementation of base64 that uses '_', '-' and '.' instead of '/', '+' and '='
    /// </summary>
    public class UrlSafeBase64 {

        /// <summary>
        /// Encodes bytes into URL safe base64 (uses _-. instead of /+=)
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
        /// Decodes bytes from base64 (Can decode url safe (_-.) or standard (/+=) b64)
        /// </summary>
        public static byte[] DecodeBytes(string B64EncodedBytes) {
            if (string.IsNullOrWhiteSpace(B64EncodedBytes)) {
                return null;
            }

            B64EncodedBytes = B64EncodedBytes.Replace('_', '/').Replace('-', '+').Replace('.', '=');

            try {
                Byte[] b = Convert.FromBase64String(B64EncodedBytes);
                return b;
            } catch (FormatException e) {
                return null;
            }
        }


    }
}
