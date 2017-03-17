using System;
using System.Collections.Generic;

namespace CryptLink {

    /// <summary>
    /// A block chain collection
    /// </summary>
    /// <typeparam name="V"></typeparam>
	public class BlockChain<V> 
        where V : Hashable{

		private Dictionary<byte[], BlockItem<V>> PendingItems = new Dictionary<byte[], BlockItem<V>>();
		private Dictionary<byte[], BlockItem<V>> CachedItems = new Dictionary<byte[], BlockItem<V>>();

        /// <summary>
        /// The owner of this chain, if specified, all blocks must be signed by the owner
        /// </summary>
        public Hash Owner { get; set; }

		/// <summary>
		/// If true, only the owner can add items to the chain
		/// </summary>
		public bool OwnerRestricted { get; set; }

		/// <summary>
		/// Maximum number of items in a given block
		/// </summary>
		public long BlockItemMaxLength { get; set; }

        /// <summary>
        /// Maximum number of items in the entire chain
        /// </summary>
        public long ChainMaxLength { get; set; }

        /// <summary>
        /// The maximum age of blocks in this chain, if a block expires, it will be removed with it's items
        /// </summary>
        public TimeSpan ChainMaxAge { get; set; }

		/// <summary>
		/// Amount of time to wait for new items to be added before generating a new block
		/// </summary>
		public TimeSpan BlockMinSpan { get; set; } 

        /// <summary>
        /// The time of the last block generation
        /// </summary>
		private DateTime LastBlockGeneration { get; set; }

        /// <summary>
        /// The time of the last item added to the block
        /// </summary>
		private DateTime LastBlockItemAdded { get; set; }

		/// <summary>
		/// Nodes that are known to accept blocks from this chain
		/// </summary>
		/// <value>The peer nodes.</value>
		private List<Hash> PeerNodes { get; set; }

        /// <summary>
        /// Stores and item in the chain
        /// </summary>
        /// <param name="Item">The item to add to the chain</param>
        /// <returns>True if the store was added</returns>
		public bool StoreItem(BlockItem<V> Item) {
            Item.Status = BlockItemStatus.Pending;
            Item.Age = DateTime.Now;

            //check if the owner signed the item
            if (OwnerRestricted) {
                throw new NotImplementedException();
            }

            byte[] key;
            if (Item.Name != null) {
                key = Item.Name.Hash.Bytes;
            } else {
                key = Item.Hash.Bytes;
            }

            var existingItem = GetItem(key);
            if (existingItem != null) {
                throw new NotImplementedException();
            }

            PendingItems.Add(key, Item);
            return true;
        }

        /// <summary>
        /// Stores or updates an item in the chain
        /// </summary>
		/// <param name="Name">The lookup name of the item</param>
		/// <param name="Value">The item to store</param>
		/// <param name="Owner">If set, only members with the same private key can update the item</param>
        /// <returns>True if the store was added</returns>
		public bool StoreItem(string Name, V Value, Hash Owner) {
            var item = new BlockItem<V>() {
                Age = DateTime.Now,
                Name = new HashableString(Name),
                Status = BlockItemStatus.Pending,
                Value = Value,
                Owner = Owner
            };

            //StoreItem(Name, item);
            throw new NotImplementedException();
        }

		/// <summary>
		/// Gets an item from the chain, if it exists
		/// </summary>
		/// <returns>The item, or null.</returns>
		/// <param name="Name">Name of the item</param>
		public BlockItem<V> GetItem(byte[] Key){

			//need to search blocks
			throw new NotImplementedException();
		}

        /// <summary>
        /// Get the BlockItemStatus of a given key
        /// </summary>
		public BlockItemStatus GetItemStatus(byte[] Key) {
            BlockItem<V> i = GetItem(Key);

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
		public bool ProcessBlock(Block<V> NewBlock) {
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
        
    }
}