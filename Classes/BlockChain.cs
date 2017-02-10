using System;
using System.Collections.Generic;

namespace CryptLink {
	public class BlockChain<T> where T : Hashable {

        /// <summary>
        /// The owner of this chain, if specified, all blocks must be signed by the owner
        /// </summary>
        public Hash Owner { get; set; }

		/// <summary>
		/// If true, only the owner can add items to the chain
		/// </summary>
		public bool OwnerRestricted {
            get {
                return Owner != (Hash)null;
            }
        }

        /// <summary>
        /// If true, this object will keep a live index of all current items
        /// </summary>
        public bool LiveIndex { get; set; }

        /// <summary>
        /// If true, all names will be converted to lowercase before storage
        /// </summary>
        public bool CaseInsensitive { get; set; }

		/// <summary>
		/// Maximum number of items in a given block
		/// </summary>
		public long MaxBlockLength { get; set; }

        /// <summary>
        /// Maximum number of items in the entire chain
        /// </summary>
        public long MaxChainLength { get; set; }

        /// <summary>
        /// The maximum age of blocks in this chain, if a block expires, it will be removed with it's items
        /// </summary>
        public TimeSpan MaxChainAge { get; set; }

		/// <summary>
		/// Amount of time to wait for new items to be added before generating a new block
		/// </summary>
		public TimeSpan MinBlockSpan { get; set; } 

        /// <summary>
        /// The time of the last block generation
        /// </summary>
		private DateTime LastBlockGeneration { get; set; }

        /// <summary>
        /// The time of the last item added to the block
        /// </summary>
		private DateTime LastBlockItemAdded { get; set; }

        /// <summary>
        /// Stores and item in the chain
        /// </summary>
        /// <returns>True if the store was added</returns>
		public bool StoreItem(string Name, BlockItem<T> Item) {
			throw new NotImplementedException();
		}

        /// <summary>
        /// Stores and item in the chain
        /// </summary>
        /// <returns>True if the store was added</returns>
		public bool StoreItem(string Name, T Value, Hash Owner) {
            var bi = new BlockItem<T>() {
                Age = DateTime.Now,
                Name = Name,
                Status = BlockItemStatus.Pending,
                Value = Value,
                Owner = Owner
            };

            throw new NotImplementedException();
        }

        public BlockItem<T> GetItem(string Name){
			//how should searching be done? By name?
			//bigger question, should this just be a dictionary/KVP?
			throw new NotImplementedException();
		}

		public BlockItemStatus GetItemStatus(string Name) {
            BlockItem<T> i = GetItem(Name);

			if(i != null) {
				return i.Status;
			} else {
				return BlockItemStatus.Unknowm;
			}
		}

        /// <summary>
        /// Process an external block and integrate it into this chain
        /// </summary>
        /// <param name="NewBlock"></param>
        /// <returns></returns>
		public bool ProcessBlock(Block<T> NewBlock) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Checks the local store of blocks and removes old blocks if necessary
		/// </summary>
		public void CleanupBlocks() {
			throw new NotImplementedException();
		}


		public void SaveChain(string LocalPath) {
			throw new NotImplementedException();
		}

		public void LoadChain(string LocalPath) {
			throw new NotImplementedException();
		}

		private Dictionary<string, BlockItem<T>> itemStack = new Dictionary<string, BlockItem<T>>(); 

        
    }
}