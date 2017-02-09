using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink.Tests {
    [TestFixture]
    public class MessageContainerTests {

        [Test]
        public void MessageContainerTest1() {

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var testContainer = new MessageContainer(
                    new Hash("Test Sender", provider),
                    new Hash("Test Receiver", provider),
                    provider
                );

                var hashLength = Hash.GetProviderByteLength(provider);
                byte[] firstHash = testContainer.GetHash(provider).HashBytes;

                Assert.NotNull(firstHash, "Computed hash has data");
                Assert.AreEqual(firstHash.Length, hashLength, "Hash length for provider");

                int binaryLength = testContainer.ToBinary().Length;
                int expectedNullLength = testContainer.ByteLength(false);
                Assert.AreEqual(binaryLength, expectedNullLength, "Binary length (null payload)");

                testContainer.Payload = BitConverter.GetBytes((int)provider);

                byte[] secondHash = testContainer.GetHash(provider).HashBytes;
                Assert.AreNotEqual(firstHash, secondHash, "Modified payload changes hash");

                byte[] binaryContainer = testContainer.ToBinary();

                binaryLength = testContainer.ToBinary().Length;
                int expectedIntLength = testContainer.ByteLength(false);
                Assert.AreEqual(binaryLength, expectedIntLength, "Binary length (int payload)");

                var deseralizedContainer = MessageContainer.FromBinary(binaryContainer, true);

                //test that the b64 version of the container is the same for the original 
                var deseralizedContainerB64 = Base64.EncodeBytes(deseralizedContainer.ToBinary());
                var testContainerB64 = Base64.EncodeBytes(testContainer.ToBinary());
                Assert.AreEqual(deseralizedContainerB64, testContainerB64, "Bytes from old and new containers are the same");

                //test that a modified hash does not parse correctly
                binaryContainer = binaryContainer.Concat(Utility.GetBytes(2)).ToArray();

                var ex = Assert.Throws<FormatException>(() => MessageContainer.FromBinary(binaryContainer, true));
                Assert.NotNull(ex);

                Assert.AreEqual(testContainer.Encrypted, deseralizedContainer.Encrypted, "Deserialized container flag Encrypted");
                Assert.AreEqual(testContainer.SenderHash.HashBytes, deseralizedContainer.SenderHash.HashBytes, "Deserialized container flag SenderHash");
                Assert.AreEqual(testContainer.ReceiverHash.HashBytes, deseralizedContainer.ReceiverHash.HashBytes, "Deserialized container flag ReceiverHash");
                Assert.AreEqual(testContainer.Provider, deseralizedContainer.Provider, "Deserialized container flag Provider");
                Assert.AreEqual(testContainer.Payload, deseralizedContainer.Payload, "Deserialized container flag Payload");
                Assert.AreEqual(testContainer.GetHash(provider).HashBytes, deseralizedContainer.GetHash(provider).HashBytes, "Deserialized container flag HashBytes");
                Assert.AreEqual(testContainer.GetHash(provider).HashByteLength(false), deseralizedContainer.GetHash(provider).HashByteLength(false), "Deserialized container flag HashByteLength");

                var r = new Random();
				testContainer.Payload = Utility.GetBytes((int)(r.NextDouble() * 1024 * 128)); //up to 128k
                binaryLength = testContainer.ToBinary().Length;
                int expectedRandomLength = testContainer.ByteLength(false);
                Assert.AreEqual(binaryLength, expectedRandomLength, "Binary length (random payload)");
            }

            
            
        }
    }
}
