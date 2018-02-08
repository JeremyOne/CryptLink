using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static CryptLink.Hash;

namespace CryptLink {
    public class MessageContainer : Hashable {
        public Hash SenderHash { get; private set; }
        public Hash ReceiverHash { get; private set; }
        public bool Encrypted { get; private set; }
        public byte[] Payload { get; private set; }
        public Cert SenderCert { get; private set; }
        public Hash.HashProvider Provider { get; private set; }

        static int intLength = sizeof(int);
        
        public MessageContainer(Hash Sender, Hash Receiver, Hash.HashProvider HashProvider, byte[] MessagePayload, Cert SenderCertificate) {
            SenderHash = Sender;
            ReceiverHash = Receiver;
            Payload = MessagePayload;
            SenderCert = SenderCertificate;
            Provider = HashProvider;

            ComputeHash(Provider, SenderCert);
        }

        public override byte[] GetHashableData() {
            return BitConverter.GetBytes((int)Provider)
                .Concat(BitConverter.GetBytes(Encrypted))
                .Concat(SenderHash.Bytes)
                .Concat(ReceiverHash.Bytes)
                .Concat(Payload)
                .ToArray();
        }
        
        public byte[] ToBinary() {
            if (ComputedHash == null) {
                throw new InvalidOperationException("A hash has not been computed for this message, can't convert to binary");
            }

            return GetHashableData()
                .Concat(ComputedHash.Bytes)
                .ToArray();
        }

        /// <summary>
        /// Non-zero index byte length
        /// </summary>
        public static int MinMessageByteLength(HashProvider Provider) {
            return intLength + (Hash.GetProviderByteLength(Provider) * 3);
        }

        public override string ToString() {
            return "Sender: '" + SenderHash.ToString() + 
                "', Receiver: '" + ReceiverHash.ToString() +
                "', Payload length: " + Payload.Length.ToString() +
                "', Hash: '" + ComputedHash?.ToString();
        }

        public static MessageContainer FromBinary(byte[] Blob, bool EnforceHashCheck, Cert SigningCert) {

            //check basic validity
            if (Blob == null) {
                throw new ArgumentNullException("Blob must not be null");
            } else if (Blob.Length < 29) {
                //4 bytes for provider (int), (32 / 8) * 3 for sender, receiver and hash bits
                throw new ArgumentOutOfRangeException("Blob is too short for any hash provider");
            }

            Hash.HashProvider provider;
                
            //get the provider type
            int providerInt = BitConverter.ToInt32(Blob, 0);
            if (Enum.IsDefined(typeof(Hash.HashProvider), providerInt)) {
                provider = (Hash.HashProvider)providerInt;
            } else {
                throw new IndexOutOfRangeException("Invalid provider type");
            }

            int hashLength = Hash.GetProviderByteLength(provider);

            //check provider specific minimum length
            int minLength = MinMessageByteLength(provider);
            if (Blob.Length < minLength) {
                throw new ArgumentOutOfRangeException("Blob is too short for specified hash provider");
            }
           
            byte[] sender = new byte[hashLength];
            byte[] receiver = new byte[hashLength];
            byte[] hash = new byte[hashLength];
            
            //parse out the hashes
            Array.Copy(Blob, intLength + 1, sender, 0, hashLength);
            Array.Copy(Blob, intLength + hashLength + 1, receiver, 0, hashLength);
            Array.Copy(Blob, Blob.Length - hashLength, hash, 0, hashLength);
           
            //get the payload
            int payloadStart = intLength + (hashLength * 2) + 1;
            int payloadLength = Blob.Length - (payloadStart + hashLength);
            byte[] payload = new byte[payloadLength];
            
            if (payloadLength > 0) {
                Array.Copy(Blob, payloadStart, payload, 0, payloadLength);
            }

            MessageContainer container = new MessageContainer(
                Hash.FromComputedBytes(sender, provider, 0),
                Hash.FromComputedBytes(receiver, provider, 0),
                provider,
                payload,
                SigningCert
            );

            //verify hash
            var newHash = container.ComputedHash;
            var oldHash = Hash.FromComputedBytes(hash, provider, Blob.Length);

            if (newHash == oldHash || EnforceHashCheck == false) {
                return container;
            } else {
                throw new FormatException("Provided hash was different than the actual hash, data is invalid or modified.");
            }

        }

    }
}
