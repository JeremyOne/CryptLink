# Roadmap Checklist

## Object Hashing Framework
- [X] CompatibleBytes - IComparable interface for sorting and comparing byte arrays
- [X] Hash - Represents a hash of an object, the hash provider, size and other metadata as needed
- [X] IHashable / Hashable - interface and abstract to be implemented on any object that will support hashing
- [X] Consistent Hash Table - Stores hashable objects in a DHT/CHT, for finding what servers any given hashed object are stored on

## Object Singing Framework
- [ ] Signature
- [ ] ISignable / Signable

## Object Caching
- [X] CacheItem - Wrapper for hashable items to be stored in an IObjectCache
- [X] IObjectCache - Interface for storing and retrieving objects by hash
- [X] DictionaryCache - Implementation of IObjectCache using a in-memory ConcurrentDictionary
- [X] LiteDBCache - Implementation of IObjectCache using LiteDB

## Communications
- [X] HTTP Server - Stores and retrieves objects
- [ ] HTTP Server - Server event hubs
- [ ] HTTP Server - Requires valid certs for clients
- [ ] HTTP Client - Swarms with peers
- [ ] HTTP Client - Manages connections and requests to servers as needed
- [ ] HTTP Client - Subscribes to server events
- [ ] HTTP Client - Requires valid certs for servers

## Config
- [X] Local load/save
- [ ] Import/Export for peer joins to swarm

## Swarm Bootstraping
- [X] X509 Cert generation
- [ ] X509 Cert revocation list blockchain
- [ ] 

## Peer Actions
- [ ] 

## Blockchain
- [X] Store objects
- [X] Build blocks
- [X] Verify and import blocks
- [ ] Join a peer's blockchain
- [ ] Exchange blocks with peers
- [ ] Peers able to swarm to many chains
