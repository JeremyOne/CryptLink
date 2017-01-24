using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink.Tests {
    [TestFixture()]
    public class UtilityTests {

        [Test()]
        public void TestUtilityGeneratonMethods() {
			var guid = Utility.GenerateCryptoGuid();
            Assert.NotNull(guid);

			var bytes = Utility.GetBytes(16);
            Assert.NotNull(bytes);
            Assert.AreEqual(bytes.Length, 16);

			var bytes64 = Utility.GetBytesB64(16);
            Assert.NotNull(bytes64);
            Assert.AreEqual(bytes64.Length, 24);

            for (int i = 1; i < 500; i++) {
				var stringRandom = Utility.GetRandomString(i);
                Assert.NotNull(stringRandom);
                Assert.AreEqual(stringRandom.Length, i);
            }
            
        }

		[Test()]
		public void TestUtilityCertCompare() {
			Assert.AreEqual(Utility.CalculatePadLength(0, 1024), 1024, "Minimum length returned");

			Assert.AreEqual(Utility.CalculatePadLength(1024, 0), 1024, "1024 returns 1024");
			Assert.AreEqual(Utility.CalculatePadLength(1025, 0), 1152, "1025 returns 1152");
			Assert.AreEqual(Utility.CalculatePadLength(1023, 0), 1024, "1023 returns 1024");

			Assert.AreEqual(Utility.CalculatePadLength(1048576, 0), 1048576, "1,048,576 returns 1,048,576");
			Assert.AreEqual(Utility.CalculatePadLength(1048577, 0), 1179648, "1,048,577 returns 1,179,648");
			Assert.AreEqual(Utility.CalculatePadLength(1048575, 0), 1048576, "1,048,575 returns 1,048,576");

			Assert.AreEqual(Utility.CalculatePadLength(0, 0), 1, "0 returns 1");

			var ex = Assert.Throws<ArgumentException>(() => Utility.CalculatePadLength(-1, 0));
			Assert.NotNull(ex);
		}

        [Test()]
        public void TestVerifyCert() {
            var testCert = Utility.GetCertFromUrl(new Uri("https://google.com"));
            Assert.True(Utility.VerifyCert(testCert, false, true, null));

            var selfSignedCert = new X509Certificate2Builder() {
                SubjectName = "Test CA",
                KeyStrength = 2048
            }.Build();

            Assert.False(Utility.VerifyCert(selfSignedCert, false, false, null), "Self-signed cert fails with strict root enforcement");
            Assert.False(Utility.VerifyCert(selfSignedCert, true, true, null), "Self-signed cert fails when checking revocation status");
            //Note: more variations of tests in X509CertTests.cs
        }
    }
}
