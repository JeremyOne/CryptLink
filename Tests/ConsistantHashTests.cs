using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink.Tests {
    [TestFixture]
    public class ConsistantHashTests {

        [Test]
        public void ConsistantHashTests1() {
            var testSize = 10000;
            var colissions = new Dictionary<Hash.HashProvider, int>();
            colissions.Add(Hash.HashProvider.SHA64, 9535);
            colissions.Add(Hash.HashProvider.SHA128, 9998);
            colissions.Add(Hash.HashProvider.SHA256, 10000);
            colissions.Add(Hash.HashProvider.SHA512, 10000);

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {

                DateTime startTime = DateTime.Now;
                var chStrings1 = new ConsistentHash<HashableString>(provider);
                Hash lastHash = new Hash();
                
                for (int i = 0; i < testSize; i++) {
                    lastHash = chStrings1.Add(new HashableString(i.ToString()), false, 1);
                }

                //smaller hash sizes may have collisions and effect the total number of items in the array
                var colisionSize = colissions[provider];

                Assert.AreEqual(chStrings1.AllNodes.Count, colisionSize, "Added all nodes in: " + (startTime - DateTime.Now).TotalMilliseconds);

                DateTime startTime2 = DateTime.Now;
                chStrings1.UpdateKeyArray();

                Assert.AreEqual(chStrings1.AllNodes.Count, colisionSize, "Updated key in: " + (startTime2 - DateTime.Now).TotalMilliseconds);
                Assert.True(chStrings1.ContainsNode(lastHash));
                
            }

        }

    }

}


