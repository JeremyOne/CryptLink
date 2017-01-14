using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

	/// <summary>
	/// Helper functions for common RNG generation tasks
	/// </summary>
    public class Crypto {
        static RNGCryptoServiceProvider RNG = new RNGCryptoServiceProvider();

        public static Guid GenerateCryptoGuid() {
            byte[] bytes = GetBytes(16);
            var g = new Guid(bytes);
            return g;
        }

        /// <summary>
        /// Generates a b64 style string of a specific length, useful for padding
        /// </summary>
        public static string GetRandomString(int StringLength) {
            var str = GetBytesB64(StringLength);

            if (str.Length >= StringLength) {
                return str.Substring(0, StringLength);
            } else {
                return str;
            }
        }

        public static byte[] GetBytes(int Length) {
            var buffer = new byte[Length];
            RNG.GetBytes(buffer);
            return buffer;
        }

        public static string GetBytesB64(int ByteLength) {
            var b = GetBytes(ByteLength);
            return Base64.EncodeBytes(b);
        }

    }
}
