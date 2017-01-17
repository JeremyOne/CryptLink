using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    class X509Certificate2Extentions {
        
        public static X509Certificate2 ToX509Certificate2(
            Org.BouncyCastle.X509.X509Certificate BouncyCert, 
            SecureRandom RandomProvider, 
            AsymmetricCipherKeyPair KeyPair) {

            var store = new Pkcs12Store();
            var certificateEntry = new X509CertificateEntry(BouncyCert);
            string friendlyName = BouncyCert.SubjectDN.ToString();

            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(KeyPair.Private), new[] { certificateEntry });

            var stream = new MemoryStream();
            var password = KeyPair.Private.GetHashCode().ToString();
            store.Save(stream, password.ToCharArray(), RandomProvider);

            //convert from stream
            var convertedCertificate =
                new X509Certificate2(
                    stream.ToArray(), password,
                    X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            return convertedCertificate;
        }

        public static bool AddCertToStore(X509Certificate2 cert, StoreName st, StoreLocation sl) {
            try {
                X509Store store = new X509Store(st, sl);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);

                store.Close();
                return true;
            } catch {
                return false;
            }
        }

        public static X509Certificate2 GetCertFromStore(StoreName st, StoreLocation sl, string SerialNumber) {
            try {
                X509Store store = new X509Store(st, sl);
                store.Open(OpenFlags.ReadWrite);
                var foundCert = store.Certificates.Find(X509FindType.FindBySerialNumber, SerialNumber, true);

                store.Close();

                if (foundCert.Count > 0) {
                    return foundCert[0];
                } else if (foundCert.Count > 1) {
                    throw new IndexOutOfRangeException("More than one cert found for the seral number: " + SerialNumber.ToString());
                } else {
                    return null;
                }

            } catch {
                return null;
            }
        }



    }
}
