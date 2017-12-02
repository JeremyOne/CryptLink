# CryptLink
A open source network that fosters free speech by providing a reliable and safe global scale network.

## Status
CryptLink is not ready to run, and currently the code here may or may not pass tests. As the project progresses expect a more strict releases.

## Roadmap
For feature breakout and completion, see [Roadmap.md](Roadmap.md)

## Primary Goals
* Provide a secure and reliable peer-to-peer-to-client messaging and data storage network
* Employ strong encryption end-to-end for all clients
* Employ strong peer to peer (server to server) encryption for all nodes
* Provide anyone that would like to start an independent network the tools to do so
* Do not rely on any public Internet resources,  (other than IP routing)
* Via bootstraping, let any client join any network without relying on public DNS
* Allow peer (server) nodes to be run on any platform supported by Mono

## Secondary Goals
* Create a reputation system for users
* Create a standard API for 3rd party apps and command line users to:
  * Get and store data
  * Send and receive messages
  * Send broadcast (multiple receiver) messages
  * Push-style notifications of new messages
  * Get network user info as provided
* Allow browsers to participate in network using local storage
* Auto-push updates to all members
* Ability to use TCP or UDP for data transfers

## Project Setup
`nuget restore -PackagesDirectory Packages`
For visual studio 2012+ Install the "NUnit 2 Test Adapter" from Tools > Extentions and Updates

## Storage types
There are three basic types of data that can be stored in the network, each with different properties

### Messages
A small object that is sent to a key through the network mesh
-Best effort only
-Receivers must confirm, or message considered unsent
-No storage assumed, only the recipient can confirm storage

### Items
A small object stored by hash, likely cached

### Blobs
Larger objects stored by hash, in slow storage

## Technology
CryptLink will use many existing technologies and ideas in new ways, including:

* TLS Trust Networks with a private root certificate authority for peer servers authentication and encryption
* HTTP(s) requests and sockets for general communication, for the most part will look like regular web browsing to an observer
* DNSSEC for public server name resolution and bootstrapping
* Client side x509 certs for end-to-end encryption and signing
* DHT (Distributed Hash Table) for storing and retrieving arbitrary objects
* Block chain for user reputation
* Unit testing (via nunit) for all major features

## Licenses
See [Licenses.md](Licenses.md)