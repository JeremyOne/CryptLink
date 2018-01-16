# Roadmap Checklist

## Object Hashing Framework
- [X] CompatibleBytes - IComparable interface for sorting and comparing byte arrays
- [X] Hash - Represents a hash of an object, the hash provider, size and other metadata as needed
- [X] IHashable / Hashable - interface and abstract to be implemented on any object that will support hashing
- [X] Consistent Hash Table - Stores hashable objects in a DHT/CHT, for finding what servers any given hashed object are stored on
- [ ] Signature - Allow hashable objects to be signed and verified

## Certificates
- [ ] Create a cache store for all known peer certs
- [ ] Find the best way to store private keys across all platforms and keep them encrypted or protected as often as possible

## Object Caching
- [X] CacheItem - Wrapper for hashable items to be stored in an IObjectCache
- [X] IObjectCache - Interface for storing and retrieving objects by hash
- [X] DictionaryCache - Implementation of IObjectCache using a in-memory ConcurrentDictionary
- [X] LiteDBCache - Implementation of IObjectCache using LiteDB

## Communications
- [X] HTTP Server - Stores and retrieves objects
- [ ] HTTP Server - Server event hubs
- [ ] HTTP Server - Requires valid certs for clients
- [ ] HTTP Client - Swarms with DHT peers
- [ ] HTTP Client - Manages connections and requests to servers as needed
- [ ] HTTP Client - Subscribes to server events
- [ ] HTTP Client - Requires valid certs for servers

## Config
- [X] Local load/save
- [X] Password protect keys in config
- [ ] Allow for key storage in OS protected store
- [ ] Import/Export for peer joins to swarm

## Swarm Bootstraping
- [X] X509 Cert generation
- [ ] X509 Custom Json Seralizer
- [X] X509 Cert revocation list blockchain
- [ ] DNS Lookups
- [ ] Authentication

## Peer Actions
- [ ] Store Item
- [ ] Retrieve Item
- [ ] Send Message
- [ ] Retrieve Message
- [ ] Swarm to blockchain

## Protocols - Implement IPeerTransports
- [ ] Https Requests - Bi-directional and uni-directional
- [ ] WebSockets
- [ ] UDP
- [ ] DNS / SEC
- [ ] SMTP
- [ ] IRC
- [ ] Transaction log files / sneakernet

## Blockchain
- [X] Store objects
- [X] Build blocks
- [X] Verify and import blocks
- [ ] Join a peer's blockchain
- [ ] Exchange blocks with peers
- [ ] Peer Reputation and blacklisting
- [ ] User Reputation chain

## Misc - Low Priority Ideas
- [ ] Add support for SHA3 https://www.nuget.org/packages/SHA3