﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    public interface IHashable {
        /// <summary>
        /// A byte array of data to be hashed
        /// </summary>
        byte[] GetHashableData();

        /// <summary>
        /// Gets the hash of this object using a specified provider, and signs it with a certificate (if provided)
        /// </summary>
        void ComputeHash(Hash.HashProvider Provider, Cert SigningCert);

        bool Verify();

        bool Verify(out string Reason);

        bool Verify(Cert SigningPublicCert);

        bool Verify(Cert SigningPublicCert, out string Reason);

        /// <summary>
        /// The computed hash of this object
        /// </summary>
        Hash ComputedHash { get; set; }

    }
}
