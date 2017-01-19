﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    class MessageContainer : Hashable {
        public Hash SenderHash { get; set; }
        public Hash ReceiverHash { get; set; }
        public Hash.HashProvider Provider { get; set; }
        public bool Encrypted { get; set; }
        public byte[] Payload { get; set; }

        static int intLength = sizeof(int);

        public MessageContainer(Hash Sender, Hash Receiver, Hash.HashProvider HashProvider) {
            SenderHash = Sender;
            ReceiverHash = Receiver;
            Provider = HashProvider;
            Payload = new byte[0];
        }

        public override byte[] HashableData() {
            return BitConverter.GetBytes((int)Provider)
                .Concat(BitConverter.GetBytes(Encrypted))
                .Concat(SenderHash.HashBytes)
                .Concat(ReceiverHash.HashBytes)
                .Concat(Payload)
                .ToArray();
        }

        public byte[] ToBinary() {
            return HashableData()
                .Concat(
                    GetHash(Provider).HashBytes
                ).ToArray();
        }

        public static int ByteCount(Hash.HashProvider ForProvider, int PayloadBytes) {
            int pl = Hash.GetHashByteLength(ForProvider);
            return intLength + (pl * 3);
        }

        /// <summary>
        /// Non-zero index byte length
        /// </summary>
        public int ByteLength(bool ZeroIndexed) {
            return ByteCount(Provider, Payload.Length);
        }

        public static MessageContainer FromBinary(byte[] Blob) {

            //check basic validity
            if (BitConverter.IsLittleEndian) {
                throw new NotImplementedException("Little Endian not yet supported");
            } else if (Blob == null) {
                throw new ArgumentNullException("Blob must not be null");
            } else if (Blob.Length < 101) {
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

            int hashLength = Hash.GetHashByteLength(provider);

            //check provider specific minimum length
            int minLength = intLength + (hashLength * sizeof(int));
            if (Blob.Length < minLength) {
                throw new ArgumentOutOfRangeException("Blob is too short for specified hash provider");
            }
           
            byte[] sender = new byte[hashLength];
            byte[] receiver = new byte[hashLength];
            byte[] hash = new byte[hashLength];

            Array.Copy(Blob, intLength, sender, 0, hashLength);
            Array.Copy(Blob, intLength + hashLength, receiver, 0, hashLength);
            Array.Copy(Blob, Blob.Length - hashLength, hash, 0, hashLength);

            //parse out the hashes
            MessageContainer container = new MessageContainer(
                new Hash(sender, provider),
                new Hash(receiver, provider),
                provider
            );
            
            //get the payload
            int payloadStart = intLength + (hashLength * 2);
            int payloadLength = Blob.Length - (payloadStart + hashLength);

            if (payloadLength > 0) {
                container.Payload = new byte[hashLength];
                Array.Copy(Blob, payloadStart, container.Payload, 0, payloadLength);
            }

            //verify hash
            if (container.GetHash(container.Provider) == hash) {
                return container;
            } else {
                throw new FormatException("Provided hash was different than the actual hash, data is invalid or modified.");
            }

        }

    }
}
