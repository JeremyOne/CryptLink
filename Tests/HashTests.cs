using NUnit.Framework;
using System;
using CryptLink;
using System.Collections.Generic;

namespace CryptLink.Tests {
	[TestFixture()]
	public class HashTests {

        [Test()]
        public void HashSeralizeTests() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = new Hash("TEST", provider);
                var h1FromBytes = Hash.FromBinaryHash(h1.HashBytes, provider);
                var h1FromB64 = Hash.FromB64(Base64.EncodeBytes(h1.HashBytes), provider);

                Assert.AreEqual(h1.HashBytes, h1FromBytes.HashBytes, "Compared Bitwise");
                Assert.True(h1 == h1FromBytes, "Compared with equality");

                Assert.AreEqual(h1.HashBytes, h1FromB64.HashBytes, "Compared Bitwise");
                Assert.True(h1 == h1FromB64, "Compared with equality");
            }
        }

        [Test()]
        public void HashingTests() {

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = new Hash("TEST", provider);
                var h2 = new Hash("TEST", provider);
                var h3 = new Hash("test", provider);
                var h4 = new Hash("", provider);

                Assert.AreEqual(h1.Provider, provider,
                    "HashProvider is set correctly");

                Assert.AreEqual(h1.HashBytes, h2.HashBytes,
                    "'TEST' and 'TEST' hashes are equal for provider: '" + provider.ToString() + "'");
                Assert.AreNotEqual(h2.HashBytes, h3.HashBytes,
                    "'TEST' and 'test' hashes are NOT equal for provider: '" + provider.ToString() + "'");

                Assert.True(h1.CompareTo(h2) == 0,
                    "Separate hashes of the same string compare returns true for provider: '" + provider.ToString() + "'");
                Assert.True(h1.CompareTo(h3) != 0,
                    "Separate hashes of the different case strings returns false for provider: '" + provider.ToString() + "'");

                Assert.AreEqual(h1.Rehash().HashBytes, h2.Rehash().HashBytes,
                    "Rehash of the same strings returns true for provider: '" + provider.ToString() + "'");

                Assert.False(h4.Valid,
                    "Hash of empty string is not valid for provider: '" + provider.ToString() + "'");

            }
        }

        [Test()]
        public void HashObjectOperatorTests() {
            foreach(Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {

                var h1 = new Hash("TEST", provider);
                var h2 = new Hash("TEST", provider);
                var h3 = new Hash("test", provider);

                //All operators (hash to hash)
                Assert.AreEqual(h1 == h2, true,
                    "Operator '==' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 == h3, false,
                    "Operator '==' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 != h2, false,
                    "Operator '!=' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 != h3, true,
                    "Operator '!=' compares correctly for provider: '" + provider.ToString() + "'");

                Assert.AreEqual(h1 > h2, false,
                    "Operator '>' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 > h3, true,
                    "Operator '>' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 < h2, false,
                    "Operator '<' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 < h3, false,
                    "Operator '<' compares correctly for provider: '" + provider.ToString() + "'");

                Assert.AreEqual(h1 >= h2, true,
                    "Operator '>=' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 >= h3, true,
                    "Operator '>=' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 <= h2, true,
                    "Operator '<=' compares correctly for provider: '" + provider.ToString() + "'");
                Assert.AreEqual(h1 <= h3, false,
                    "Operator '<=' compares correctly for provider: '" + provider.ToString() + "'");

            }
        }

        [Test()]
        public void HashBinaryOperatorTests() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                var h1 = new Hash("TEST", provider);
                var h2 = new Hash("TEST", provider);
                var h3 = new Hash("test", provider);

                //All operators (hash to binary)
                Assert.AreEqual(h1 == h2.HashBytes, true,
                    "Operator '==' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 == h3.HashBytes, false,
                    "Operator '==' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 != h2.HashBytes, false,
                    "Operator '!=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 != h3.HashBytes, true,
                    "Operator '!=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");

                Assert.AreEqual(h1 > h2.HashBytes, false,
                    "Operator '>' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 > h3.HashBytes, true,
                    "Operator '>' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 < h2.HashBytes, false,
                    "Operator '<' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 < h3.HashBytes, false,
                    "Operator '<' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");

                Assert.AreEqual(h1 >= h2.HashBytes, true,
                    "Operator '>=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 >= h3.HashBytes, true,
                    "Operator '>=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 <= h2.HashBytes, true,
                    "Operator '<=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
                Assert.AreEqual(h1 <= h3.HashBytes, false,
                    "Operator '<=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
            }
        }

        [Test()]
        public void HashSortingTests() {
            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {
                
                var h1 = new Hash("1", provider);
                var h2 = new Hash("2", provider);
                var h3 = new Hash("3", provider);

                var hList = new List<Hash>();
                hList.Add(h1);
                hList.Add(h2);
                hList.Add(h3);

                Assert.IsNotEmpty(hList);
                Assert.AreEqual(hList.Count, 3);

                hList.Sort();

            }
        }

    }

}

