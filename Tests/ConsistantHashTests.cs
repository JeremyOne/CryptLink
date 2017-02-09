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
            var testSize = 5000;

            foreach (Hash.HashProvider provider in Enum.GetValues(typeof(Hash.HashProvider))) {

                DateTime startTime = DateTime.Now;
                var chStrings1 = new ConsistentHash<HashableString>(provider);
                Hash lastHash = new Hash();
                
                for (int i = 0; i < testSize; i++) {
                    var h = new HashableString(i.ToString());
                    lastHash = chStrings1.Add(h, false, 0);
                }

                Assert.AreEqual(chStrings1.AllNodes.Count, testSize, "Added all nodes in: " + (DateTime.Now - startTime).TotalMilliseconds + "MS");

                DateTime startTime2 = DateTime.Now;
                chStrings1.UpdateKeyArray();

                Assert.AreEqual(chStrings1.AllNodes.Count, testSize, "Updated key in: " + (DateTime.Now - startTime2).TotalMilliseconds + "MS");
                Assert.True(chStrings1.ContainsNode(lastHash));
                
            }

        }

    }

}


