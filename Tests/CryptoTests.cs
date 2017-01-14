using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink.Tests {
    [TestFixture()]
    public class CryptoTests {

        [Test()]
        public void TestCryptoGeneratonMethods() {
            var guid = Crypto.GenerateCryptoGuid();
            Assert.NotNull(guid);

            var bytes = Crypto.GetBytes(16);
            Assert.NotNull(bytes);
            Assert.AreEqual(bytes.Length, 16);

            var bytes64 = Crypto.GetBytesB64(16);
            Assert.NotNull(bytes64);
            Assert.AreEqual(bytes64.Length, 24);

            for (int i = 1; i < 500; i++) {
                var stringRandom = Crypto.GetRandomString(i);
                Assert.NotNull(stringRandom);
                Assert.AreEqual(stringRandom.Length, i);
            }
            
        }
    }
}
