using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink
{
    //Dev notes:
    //based on https://code.google.com/p/consistent-hash/  (Lesser GPL)
    //Added comments
    //Modified to use Hash object instead of Int to store hash data in, this allows for any length hash
    // -Keys now use Hash or byte[] instead of a MurmerHash2 of a string
    // -See Hash.cs for more details on how comparisons of hashes (ultimately a byte[]) are performed
    //Modified to allow for weighted distribution (replication) of peers, this allows for unequal weighting of nodes


    /// <summary>
    /// A hash table designed mostly for selecting hosts for content,
    /// works by putting content in the host with the next most similar hash
    /// </summary>
    /// <typeparam name="T">Type of object to store, must be an abstract class of Hashable</typeparam>
    class ConsistentHash<T> {
        SortedDictionary<Hash, T> circle = new SortedDictionary<Hash, T>();
        SortedDictionary<Hash, T> unreplicatedNodes = new SortedDictionary<Hash, T>();
        Dictionary<Hash, int> replicationWeights = new Dictionary<Hash, int>();
        Hash.HashProvider Provider = Hash.HashProvider.SHA256;

        Hash[] ayKeys = null;    //cache the ordered keys for better performance

        public ConsistentHash(Hash.HashProvider _Provider){
            Provider = _Provider;
        }

        private static bool IsHashable(object obj) {
            return obj.GetType().IsSubclassOf(typeof(Hashable));
        }

        /// <summary>
        /// Gets a list of all nodes, this is inefficient and should not be used unless you need one instance of every node for enumeration
        /// </summary>
        public List<T> AllNodes {
            get {
                return unreplicatedNodes.Values.ToList();
            }
        }

        /// <summary>
        /// Gets a Hash object from a object that inherits from Hashable
        /// </summary>
        /// <param name="fromObject">Object to hash</param>
        public Hash GetHash(object fromObject) {
            return GetHash(fromObject, Provider);
        }

        /// <summary>
        /// Gets a Hash object from a object that inherits from Hashable
        /// </summary>
        /// <param name="fromObject">Object to hash</param>
        /// <param name="Provider">The hash provider to hash with</param>
        public static Hash GetHash(object fromObject, Hash.HashProvider Provider) {
            if(fromObject == null){
                throw new ArgumentException("Object can't be null");
            } else if (IsHashable(fromObject)) {
                var h = (Hashable)fromObject;
                return h.GetHash(Provider);
            } else {
                throw new ArgumentException("Provided object does not abstract from Hashable, can't use this interface.");
            }
        }

        /// <summary>
        /// Adds a node, runs replication
        /// </summary>
        /// <param name="node">Item to store, must be abstracted from Hashable</param>
        /// <param name="updateKeyArray">If enabled updates the key cache</param>
        /// <param name="ReplicationWeight">Nodes are added multiple times to the namespace to allow for better 
        /// distribution of peers in the address space, the higher the weight the more likely the node will be found while searching</param>
        /// <returns>The first hash of the node (If ReplicationWeight is more than 1, there is more than one hash of the item in the table)</returns>
        public Hash Add(T node, bool updateKeyArray, int ReplicationWeight) {
            var Hash = GetHash(node);
            Add(node, Hash, updateKeyArray, ReplicationWeight);
            return Hash;
        }

        /// <summary>
        /// Adds a node, runs replication
        /// </summary>
        /// <param name="node">Item to store</param>
        /// <param name="nodeHash">Hash of the item. If the node inherits from Hashable, use the overload that does not require this</param>
        /// <param name="updateKeyArray">If enabled updates the key cache</param>
        /// <param name="ReplicationWeight">Nodes are added multiple times to the namespace to allow for better 
        /// distribution of peers in the address space, the higher the weight the more likely the node will be found while searching</param>
        public void Add(T node, Hash nodeHash, bool updateKeyArray, int ReplicationWeight) {
            
            for (int i = 0; i < ReplicationWeight; i++) {
                var rHash = nodeHash.Rehash(i);
                circle[rHash] = node;
            }
            
            replicationWeights.Add(nodeHash, ReplicationWeight);
            unreplicatedNodes.Add(nodeHash, node);

            if (updateKeyArray) {
                ayKeys = circle.Keys.ToArray();
            }
        }

        /// <summary>
        /// Removes node and all replicas
        /// </summary>
        /// <param name="node"></param>
        public void Remove(T node, Hash nodeHash) {
            int replicationWeight = replicationWeights[nodeHash];

            for (int i = 0; i < replicationWeight; i++) {
                Hash newHash = nodeHash.Rehash(i);
                if (!circle.Remove(newHash)) {
                    throw new Exception("Error removing replicated hashes, this should only happen if: " +
                    "1. There was a hash collision, 2. The key array was modified outside of this logic");
                }
            }

            unreplicatedNodes.Remove(nodeHash);

            ayKeys = circle.Keys.ToArray();
        }

        /// <summary>
        /// return the index of first item that is greater than 'Value'
        /// </summary>
        /// <param name="ay">The ordered dictionary to search</param>
        /// <returns>if there are no nodes, or search fails, will return 0</returns>
        int First_ge(Hash Value) {
            int begin = 0;
            int end = ayKeys.Length - 1;

            if (ayKeys[end] < Value || ayKeys[0] > Value) {
                return 0;
            }

            int mid = begin;
            while (end - begin > 1) {
                mid = (end + begin) / 2;
                if (ayKeys[mid] >= Value) {
                    end = mid;
                } else {
                    begin = mid;
                }
            }

            if (ayKeys[begin] > Value || ayKeys[end] < Value) {
                throw new Exception("No nodes in the search space, this should not happen");
            }

            return end;
        }

        public T GetNode(byte[] key) {
            Hash h = Hash.FromBinaryHash(key, null);
            int first = First_ge(h);
            return circle[ayKeys[first]];
        }

        public T GetNode(Hash key) {
            int first = First_ge(key);
            return circle[ayKeys[first]];
        }

        public bool ContainsNode(Hash key) {
            //unsure of the efficiency of this
            return ayKeys.Contains(key);
        }

    }
}
