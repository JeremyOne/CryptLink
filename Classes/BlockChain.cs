using System;
using System.Collections.Generic;

namespace CryptLink {
	public class BlockChain<T> : Hashable {
		public Hash Owner { get; set; }

		/// <summary>
		/// If true, only the owner can add items to the chain
		/// </summary>
		public bool OwnerRestricted { get; set; }

		/// <summary>
		/// Maximum number of items in a given block
		/// </summary>
		public long MaxBlockLength { get; set; }

		/// <summary>
		/// The maximum age of blocks in this chain, if a block expires, it will be removed with it's items
		/// </summary>
		public TimeSpan MaxChainAage { get; set; }

		/// <summary>
		/// Amount of time to wait for new items to be added before generating a new block
		/// </summary>
		public TimeSpan MinBlockSpan { get; set; } 

		/// <summary>
		/// Items that are pending for the next block
		/// </summary>
		private List<T> PendingItems { get; set; }

		private DateTime LastBlockGeneration { get; set; }
		private DateTime LastBlockItemAdd { get; set; }


		public bool AddItem(T Item){
			throw new NotImplementedException();
		}

		public bool FindItem(){
			//how should searching be done? By name?
			//bigger question, should this just be a dictionary/KVP?
			throw new NotImplementedException();
		}


		public bool ProcessExternalBlock(Block<T> NewBlock) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Checks the local store of blocks and removes old blocks if necessary
		/// </summary>
		public void CleanupBlocks() {
			throw new NotImplementedException();
		}

        public override byte[] HashableData() {
            throw new NotImplementedException();
        }
    }
}

