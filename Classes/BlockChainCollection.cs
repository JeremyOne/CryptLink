using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace CryptLink {
    class BlockChainCollection {
        public Dictionary<Hash, IBlockChain> Chains = new Dictionary<Hash, IBlockChain>(); 
        
    }
}
