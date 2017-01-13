using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

	/// <summary>
	/// Helper functions for common crypto tasks
	/// </summary>
    class Crypto {
        static RNGCryptoServiceProvider RNG = new RNGCryptoServiceProvider();

        public static Guid GenerateCryptoGuid() {
            byte[] bytes = GetBytes(16);
            var g = new Guid(bytes);
            return g;
        }

        public static string GetRandomString(int StringLength) {
            return GetBytesB64(StringLength * 6);
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
