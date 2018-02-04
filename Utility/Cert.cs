using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using static CryptLink.Hash;

namespace CryptLink {

    /// <summary>
    /// A wrapper for the x509Certificate2 with many helpers
    /// </summary>
    public class Cert : Hashable {
        
        /// <summary>
        /// The base 64 encoded bytes that comprise the certificate
        /// </summary>
        public string CertificateBase64 { get; set; }

        /// <summary>
        /// The path to where the cert is stored in the OS's secure storage
        /// </summary>
        public string ProtectedStoragePath { get; set; }

        /// <summary>
        /// If true, the b64 version of the certificate will be encrypted with the specified password
        /// </summary>
        public bool PasswordEncrypt { get; set; }

        /// <summary>
        /// The hash provider for this object
        /// </summary>
        public HashProvider Provider { get; set; }

        /// <summary>
        /// The password to encrypt/decrypt the certificate with
        /// </summary>
        [JsonIgnore]
        public SecureString EncryptionPassword { get; set; }

        /// <summary>
        /// For deserializing
        /// </summary>
        public Cert() { }

        /// <summary>
        /// Loads the certificate using a SecureString for decryption
        /// This function is only needed when the certificate is encrypted with a password.
        /// </summary>
        public void LoadCertificate(SecureString EncryptionPassword, HashProvider Provider) {
            this.EncryptionPassword = EncryptionPassword;
            this.Provider = Provider;
            LoadCertificate(Provider);
        }

        [OnDeserialized]
        internal void OnDeseralized(StreamingContext context) {
            //after deserialization, load the certificate
            if (PasswordEncrypt == false) {
                LoadCertificate(Provider);
            }
        }
        
        private void LoadCertificate(HashProvider Provider) {
            if (PasswordEncrypt) {
                if (EncryptionPassword == null) {
                    throw new Exception("No decryption password was specified, can't encrypt the certificate");
                }

                _certificate = new X509Certificate2(UrlSafeBase64.DecodeBytes(CertificateBase64), EncryptionPassword);
            }

            if (ProtectedStoragePath != null) {
                throw new NotImplementedException("Todo: implement OS storage");
            }

            if (!PasswordEncrypt && ProtectedStoragePath == null) {
                _certificate = new X509Certificate2(UrlSafeBase64.DecodeBytes(CertificateBase64));
            }

            ComputeHash(Provider);

        }

        public Cert(X509Certificate2 Certificate) {
            this._certificate = Certificate;
            ComputeHash(Provider);
            SeralizeCertificate();
        }

        public Cert(X509Certificate2 Certificate, SecureString EncryptionPassword) {
            this._certificate = Certificate;
            this.EncryptionPassword = EncryptionPassword;
            this.PasswordEncrypt = true;
            ComputeHash(Provider);
            SeralizeCertificate();
        }

        private X509Certificate2 _certificate { get; set; }

        public string Thumbprint {
            get {
                CheckCertificate();
                return ComputedHash.ToString();
            }
        }

        public bool CheckCertificate() {
            if (_certificate == null) {
                throw new NullReferenceException("The certificate does not exist, make sure it is accessible, the decryption password is correct");
            }

            return true;
        }

        [OnSerializing]
        public void SeralizeCertificate() {
            CheckCertificate();

            if (PasswordEncrypt) {
                if (EncryptionPassword == null) {
                    throw new Exception("No decryption password was specified, can't encrypt the certificate");
                }

                CertificateBase64 = UrlSafeBase64.EncodeBytes(_certificate.Export(X509ContentType.Pkcs12, EncryptionPassword));
            }

            if (ProtectedStoragePath != null) {
                throw new NotImplementedException("Todo: implement OS storage");
            }

            if (!PasswordEncrypt && ProtectedStoragePath == null) {
                CertificateBase64 = UrlSafeBase64.EncodeBytes(_certificate.Export(X509ContentType.Pkcs12));
            }

        }

        [JsonIgnore]
        public PublicKey PublicKey {
            get {
                return _certificate.PublicKey;
            }
        }

        public bool HasPrivateKey {
            get {
                return _certificate.HasPrivateKey;
            }
        }

        public int KeyLength {
            get {
                return _certificate.PublicKey.Key.KeySize;
            }
        }

        /// <summary>
        /// Re-parses an X509Certificate2 to only contain the public key
        /// </summary>
        public Cert RemovePrivateKey() {
            if (_certificate == null) {
                return null;
            } else {
                return new Cert(new X509Certificate2(_certificate.RawData));
            }

        }

        /// <summary>
        /// Use this cert to sign a hash
        /// </summary>
        /// <param name="Hash">The hash to sign</param>
        /// <param name="Provider">The provider to use</param>
        public void SignHash(Hash Hash, HashProvider Provider) {
            if (_certificate.HasPrivateKey && Hash.Bytes != null) {
                var csp = (RSACryptoServiceProvider)_certificate.PrivateKey;
                Hash.SignatureBytes = csp.SignHash(Hash.Bytes, Hash.GetOIDForProvider(Provider));
                Hash.SignatureCertHash = this.ComputedHash.Bytes;
            } else {
                throw new NullReferenceException("No private key");
            }
        }

        public override byte[] GetHashableData() {
            if (_certificate == null) {
                //possible improvement: get from web request
                throw new NullReferenceException("Certificate not set");
            } else {
                return _certificate.PublicKey.EncodedKeyValue.RawData;
            }
        }
    }
}
