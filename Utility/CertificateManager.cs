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

namespace CryptLink {
    public class CertificateManager {

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
        /// The password to encrypt/decrypt the certificate with
        /// </summary>
        [JsonIgnore]
        public SecureString EncryptionPassword { get; set; }

        public CertificateManager() { }

        /// <summary>
        /// Loads the certificate using a SecureString for decryption
        /// This function is only needed when the certificate is encrypted with a password.
        /// </summary>
        public void LoadCertificate(SecureString EncryptionPassword) {
            this.EncryptionPassword = EncryptionPassword;
            LoadCertificate();
        }

        [OnDeserialized]
        internal void OnSeralized(StreamingContext context) {
            //after deserialization, load the certificate
            if (PasswordEncrypt == false) {
                LoadCertificate();
            }
        }

        private void LoadCertificate() {
            if (PasswordEncrypt) {
                if (EncryptionPassword == null) {
                    throw new Exception("No decryption password was specified, can't encrypt the certificate");
                }

                _certificate = new X509Certificate2(Base64.DecodeBytes(CertificateBase64), EncryptionPassword);
            }

            if (ProtectedStoragePath != null) {
                throw new NotImplementedException("Todo: implement OS storage");
            }

            if (!PasswordEncrypt && ProtectedStoragePath == null) {
                _certificate = new X509Certificate2(Base64.DecodeBytes(CertificateBase64));
            }
        }

        public CertificateManager(X509Certificate2 Certificate) {
            this._certificate = Certificate;
            SeralizeCertificate();
        }

        public CertificateManager(X509Certificate2 Certificate, SecureString EncryptionPassword) {
            this._certificate = Certificate;
            this.EncryptionPassword = EncryptionPassword;
            this.PasswordEncrypt = true;
            SeralizeCertificate();
        }

        private X509Certificate2 _certificate { get; set; }

        public string Thumbprint {
            get {
                CheckCertificate();
                return _certificate.Thumbprint;
            }
        }

        public bool CheckCertificate() {
            if (_certificate == null) {
                throw new NullReferenceException("The certificate does not exist, make sure it is accessible, the decryption password is correct");
            }

            return true;
        }

        public void SeralizeCertificate() {
            CheckCertificate();

            if (PasswordEncrypt) {
                if (EncryptionPassword == null) {
                    throw new Exception("No decryption password was specified, can't encrypt the certificate");
                }

                CertificateBase64 = Base64.EncodeBytes(_certificate.Export(X509ContentType.Pkcs12, EncryptionPassword));
            }

            if (ProtectedStoragePath != null) {
                throw new NotImplementedException("Todo: implement OS storage");
            }

            if (!PasswordEncrypt && ProtectedStoragePath == null) {
                CertificateBase64 = Base64.EncodeBytes(_certificate.Export(X509ContentType.Pkcs12));
            }

        }

    }
}
