using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {

	/// <summary>
	/// Helper functions for common RNG generation and other tasks
	/// </summary>
    public class Utility {

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

		/// <summary>
		/// Calculates padding based on powers of 2
		/// First power of two greater than the length, then subtract blocks of Pow2 / 8
		/// </summary>
		/// <returns>The desired pad.</returns>
		public static long CalculatePadLength(long ActualLength, long MinLength){

			if(ActualLength < 0) {
				throw new ArgumentException("ActualLength must be a positive number");
			}

			if(ActualLength < MinLength) {
				return MinLength;
			}

			long pow2 = 2;

			//get the first power of 2 that this message is shorter than
			while(pow2 < ActualLength) {
				pow2 += pow2;
			}

			//
			long lastPow = pow2 / 2;
			long blockSize = lastPow / 8;
			long padSize = lastPow + blockSize;

			while(padSize < ActualLength) {
				padSize += blockSize;
			}

			return padSize;
		}

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

        /// <summary>
        /// A slightly more robust x509 cert verifier for quick testing of certs
        /// </summary>
        /// <param name="Cert">Certificate to verify</param>
        /// <param name="AllowUnknownCA">If true, Allows Unknown Certificate Authority</param>
        /// <param name="CheckRevocationStatus">If true the revocation status will not be checked, 
        /// generally revocation can't be checked for custom CAs are used since they don't run a revocation server</param>
        /// <param name="CustomCA">If provided, added to the ExtraStore for CA search</param>
        /// <param name="Intermediates">Other intermediate certs to include for checking the chain</param>
        /// <returns>True if valid</returns>
        public static bool VerifyCert(X509Certificate2 Cert, bool AllowUnknownCA, X509RevocationMode CheckRevocationMode,
            X509Certificate2 CustomCA, params X509Certificate2[] Intermediates) {

            X509Chain chain = new X509Chain();
            chain.ChainPolicy = new X509ChainPolicy();

            if (AllowUnknownCA) {
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            }
            
            chain.ChainPolicy.RevocationMode = CheckRevocationMode;

            if (CustomCA != null) {
                chain.ChainPolicy.ExtraStore.Add(CustomCA);
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            }

            if (Intermediates.Length > 0) {
                chain.ChainPolicy.ExtraStore.AddRange(Intermediates);
            }

            string log = "";
            try {
                var chainBuilt = chain.Build(Cert);
                log += (string.Format("Chain built: {0}. ", chainBuilt));

                if (chainBuilt == false) {
                    foreach (X509ChainStatus chainStatus in chain.ChainStatus) {
                        log += (string.Format("Chain error, status: {0}, Info: {1}. ", chainStatus.Status, chainStatus.StatusInformation));
                    }

                    return false;
                } else {

                    if (CustomCA != null) {
                        //check the CA manually to avoid the need to install it in the computer's root store
                        var chainCA = chain.ChainElements[chain.ChainElements.Count - 1];

                        if (chainCA.Certificate.Thumbprint == CustomCA.Thumbprint) {
                            return true;
                        } else {
                            return false;
                        }
                    } else {
                        return true;
                    }

                }

            } catch (Exception ex) {
                log += (ex.ToString());
                return false;
            }


        }

        /// <summary>
        /// Gets the x509 certificate from a Uri
        /// </summary>
        /// <returns></returns>
        public static X509Certificate2 GetCertFromUrl(Uri Uri) {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Uri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                X509Certificate cert = request.ServicePoint.Certificate;
                X509Certificate2 cert2 = new X509Certificate2(cert);

                return cert2;
            } catch (Exception ex) {
                return null;
            }
            
        }

        /// <summary>
        /// Re-parses an X509Certificate2 to only contain the public key
        /// </summary>
        public static X509Certificate2 GetPublicKey(X509Certificate2 FromCert) {
            return new X509Certificate2(FromCert.RawData);
        }

        /// <summary>
        /// Signs any Hashable item
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="HashableItem">Object to sign</param>
        /// <param name="Provider">Hash provider</param>
        /// <param name="SigningCert">The cert to sign with</param>
        /// <returns>A</returns>
        public static byte[] Sign<T>(T HashableItem, Hash.HashProvider Provider, X509Certificate2 SigningCert) where T : Hashable {
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)SigningCert.PrivateKey;
            byte[] hash = HashableItem.GetHash(Provider).Bytes;
            var providerOID = Hash.GetOIDForProvider(Provider);
            return csp.SignHash(hash, providerOID);
        }


        /// <summary>
        /// Verifies any Hashable item against a key and signature
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="HashableItem">Item to verify</param>
        /// <param name="signature">Signature Bytes</param>
        /// <param name="Provider">Hash Provider</param>
        /// <param name="VerifyCert">The cert to verify against</param>
        /// <returns></returns>
        public static bool Verify<T>(T HashableItem, byte[] signature, Hash.HashProvider Provider, X509Certificate2 VerifyCert) where T : Hashable {
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)VerifyCert.PublicKey.Key;
            byte[] hash = HashableItem.GetHash(Provider).Bytes;
            var providerOID = Hash.GetOIDForProvider(Provider);
            return csp.VerifyHash(hash, providerOID, signature);
        }


        /// <summary>
        /// Calculates a score on the scale of 0.0 - 1.0 based on 
        /// </summary>
        /// <param name="InputValue"></param>
        /// <param name="Max"></param>
        /// <param name="Invert">Makes the scale 1 to 0 instead of 0 to 1</param>
        /// <returns></returns>
        public static double GetRange(double InputValue, double ValueMax) {
            return Math.Min(InputValue, ValueMax) / ValueMax;
        }

        /// <summary>
        /// Gets a temporary file name appropriate for this project
        /// </summary>
        /// <param name="FileExtention"></param>
        /// <returns></returns>
        public static string GetTempFilePath(string FileExtention) {
            return $"temp-{DateTime.Now.Ticks.ToString()}.{FileExtention}";
        }

    }
}
