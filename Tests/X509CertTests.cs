using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink.Tests {

    [TestFixture]
    public class TestX509CertBuilder{
        [Test]
        public void TestX509CertBuilderTests() {

            var ca1 = new X509Certificate2Builder { SubjectName = "CN=Test CA1" }.Build();
            Assert.True(Utility.VerifyCert(ca1, true, X509RevocationMode.NoCheck, null), "CA1 is valid (AllowUnknownCertificateAuthority = true)");

            var intermediate1 = new X509Certificate2Builder { SubjectName = "CN=Intermediate 1", Issuer = ca1 }.Build();
            Assert.False(Utility.VerifyCert(intermediate1, false, X509RevocationMode.NoCheck, null), "Intermediate 1 is not valid when CA not provided, and AllowUnknownCA is false");
            Assert.True(Utility.VerifyCert(intermediate1, false, X509RevocationMode.NoCheck, ca1), "Intermediate 1 is valid with: CA1");

            var intermediate2 = new X509Certificate2Builder { SubjectName = "CN=Intermediate 2", Issuer = intermediate1 }.Build();
            Assert.True(Utility.VerifyCert(intermediate2, false, X509RevocationMode.NoCheck, ca1, intermediate1), "Intermediate 2 is valid with: CA1.Intermediate1");

            var cert1 = new X509Certificate2Builder { SubjectName = "CN=Test 1", Issuer = intermediate1, Intermediate = false }.Build();
            Assert.True(Utility.VerifyCert(cert1, false, X509RevocationMode.NoCheck, ca1, intermediate1), "Cert 1 is valid with: CA1.intermediate1");
            Assert.False(Utility.VerifyCert(cert1, false, X509RevocationMode.NoCheck, ca1, intermediate2), "Cert 1 is NOT valid checked against intermediate 2");

            var cert2 = new X509Certificate2Builder { SubjectName = "CN=Test 2", Issuer = intermediate2 }.Build();
            Assert.True(Utility.VerifyCert(cert2, false, X509RevocationMode.NoCheck, ca1, intermediate1, intermediate2), "Cert 2 is valid with: CA1.intermediate1.intermediate2");

            var cert3 = new X509Certificate2Builder { SubjectName = "CN=Test 3", Issuer = cert2 }.Build();
            Assert.True(Utility.VerifyCert(cert3, false, X509RevocationMode.NoCheck, ca1, intermediate1, intermediate2, cert2), "Cert 3 is valid with: CA1.intermediate1.intermediate2.Cert2");


            var ca2 = new X509Certificate2Builder { SubjectName = "CN=Test CA2" }.Build();
            Assert.True(Utility.VerifyCert(ca2, true, X509RevocationMode.NoCheck, null), "CA2 is valid (AllowUnknownCertificateAuthority = true)");

            Assert.False(Utility.VerifyCert(intermediate1, false, X509RevocationMode.NoCheck, ca2), "Intermediate 1A is NOT valid and checked against CA2");

            var invalidCert3 = new X509Certificate2Builder {
                SubjectName = "CN=Invalid 1",
                IssuerName = ca1.SubjectName.Name,
                IssuerPrivateKey = ca2.PrivateKey
            }.Build();

            Assert.False(Utility.VerifyCert(invalidCert3, false, X509RevocationMode.NoCheck, ca2), "Cert 3 is NOT valid when generated with incorrect issuer name");

            var invalidCert4 = new X509Certificate2Builder {
                SubjectName = "CN=Invalid 2",
                Issuer = ca2,
                NotBefore = DateTime.Now.AddDays(1)
            }.Build();

            Assert.False(Utility.VerifyCert(invalidCert4, false, X509RevocationMode.NoCheck, ca2), "Cert 4 is NOT valid with future date");

        }
    }
}
