using NUnit.Framework;
using System;
using CryptLink;

namespace CryptLinkService {
	[TestFixture()]
	public class HashTests {
		[Test()]
		public void TestCase() {

			foreach(HashProviders provider in Enum.GetValues(typeof(HashProviders))) {
				var h1 = new Hash("TEST", provider);
				var h2 = new Hash("TEST", provider);
				var h3 = new Hash("test", provider);
				var h4 = new Hash("", provider);

				Assert.AreEqual(h1.Provider, provider, 
					"HashProvider is set correctly");

				Assert.AreEqual(h1.Hashed, h2.Hashed, 
					"'TEST' and 'TEST' hashes are equal for provider: '" + provider.ToString() + "'");
				Assert.AreNotEqual(h2.Hashed, h3.Hashed, 
					"'TEST' and 'test' hashes are NOT equal for provider: '" + provider.ToString() + "'");

				Assert.AreEqual(h1.CompareTo(h2), true, 
					"Separate hashes of the same string compare returns true for provider: '" + provider.ToString() + "'");
				Assert.AreEqual(h1.CompareTo(h3), true,
                    "Separate hashes of the similar strings returns false for provider: '" + provider.ToString() + "'");

				Assert.AreEqual(h1.Rehash(99).Hashed, h2.Rehash(99).Hashed, 
					"Rehash of the same strings returns true for provider: '" + provider.ToString() + "'");

				Assert.AreEqual(h4.Valid, false, 
					"Hash of empty string is not valid for provider: '" + provider.ToString() + "'");

				//All operators hash to hash
				Assert.AreEqual(h1 == h2, true, 
					"Operator '==' compares correctly for provider: '" + provider.ToString() + "'");
				Assert.AreEqual(h1 == h2, true, 
					"Operator '!=' compares correctly for provider: '" + provider.ToString() + "'");
				Assert.AreEqual(h1 == h2, false, 
					"Operator '!=' compares correctly for provider: '" + provider.ToString() + "'");
				Assert.AreEqual(h1 > h3, true, 
					"Operator '>' compares correctly for provider: '" + provider.ToString() + "'");
				Assert.AreEqual(h1 >= h2, true, 
					"Operator '>=' compares correctly for provider: '" + provider.ToString() + "'");
				Assert.AreEqual(h1 < h2, true, 
					"Operator '<' compares correctly for provider: '" + provider.ToString() + "'");
				Assert.AreEqual(h1 <= h2, true, 
					"Operator '<=' compares correctly for provider: '" + provider.ToString() + "'");
				

				//All operators hash to binary
				Assert.AreEqual(h1 == h2.Hashed, true, 
					"Operator '==' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
				Assert.AreEqual(h1 == h2.Hashed, true, 
					"Operator '!=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
				Assert.AreEqual(h1 == h2.Hashed, false, 
					"Operator '!=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
				Assert.AreEqual(h1 > h3.Hashed, true, 
					"Operator '>' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
				Assert.AreEqual(h1 >= h2.Hashed, true, 
					"Operator '>=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
				Assert.AreEqual(h1 < h2.Hashed, true, 
					"Operator '<' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
				Assert.AreEqual(h1 <= h2.Hashed, true, 
					"Operator '<=' compares correctly for provider (hash to binary): '" + provider.ToString() + "'");
				


				//All constructors, invalid objects
				//All compare types
				//Sorting

			}


		}
	}
}

