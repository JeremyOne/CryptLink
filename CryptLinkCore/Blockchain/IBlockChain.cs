using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    interface IBlockChain {
        /// <summary>
        /// The owner of this chain, if specified, all blocks must be signed by the owner
        /// </summary>
        Hash Owner { get; set; }

        /// <summary>
        /// If true, only the owner can add items to the chain
        /// </summary>
        bool OwnerRestricted { get; set; }

        /// <summary>
        /// Maximum number of items in a given block
        /// </summary>
        long BlockItemMaxLength { get; set; }

        /// <summary>
        /// Maximum number of items in the entire chain
        /// </summary>
        long ChainMaxLength { get; set; }

        /// <summary>
        /// The maximum age of blocks in this chain, if a block expires, it will be removed with it's items
        /// </summary>
        TimeSpan ChainMaxAge { get; set; }

        /// <summary>
        /// Amount of time to wait for new items to be added before generating a new block
        /// </summary>
        TimeSpan BlockMinSpan { get; set; }

        /// <summary>
        /// The time of the last block generation
        /// </summary>
        DateTime LastBlockGeneration { get; set; }

        /// <summary>
        /// The time of the last item added to the block
        /// </summary>
		DateTime LastBlockItemAdded { get; set; }

        /// <summary>
        /// Nodes that are known to accept blocks from this chain
        /// </summary>
        /// <value>The peer nodes.</value>
        List<Hash> PeerNodes { get; set; }

        /// <summary>
        /// Stores and item in the chain
        /// </summary>
        /// <param name="Item">The item to add to the chain</param>
        /// <returns>True if the store was added</returns>
		bool StoreItem<V>(V Item) where V : Hashable;

        /// <summary>
        /// Stores or updates an item in the chain
        /// </summary>
		/// <param name="Name">The lookup name of the item</param>
		/// <param name="Value">The item to store</param>
		/// <param name="Owner">If set, only members with the same private key can update the item</param>
        /// <returns>True if the store was added</returns>
		bool StoreItem<V>(string Name, V Value, Hash Owner) where V : Hashable;

        /// <summary>
        /// Gets an item from the chain, if it exists
        /// </summary>
        V GetItem<V>(byte[] Key) where V : Hashable;

        /// <summary>
        /// Get the BlockItemStatus of a given key
        /// </summary>
		BlockItemStatus GetItemStatus(byte[] Key);

        /// <summary>
        /// Process an external block and integrate it into this chain
        /// </summary>
        /// <param name="NewBlock"></param>
        /// <returns></returns>
		bool ProcessBlock<V>(Block<V> NewBlock) where V : Hashable;

        /// <summary>
        /// Checks the local store of blocks and removes old blocks if necessary
        /// </summary>
        void CleanupBlocks();

        void SaveChain(string LocalPath);

        void LoadChain(string LocalPath);

    }
}
