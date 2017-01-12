# CryptLink
A peer-to-peer messaging and data storage network that uses client and server side encryption.

### First Goals
* Provide a secure and reliable network of storage and messaging to anyone participating
* All messages and storage to may be end-to-end encrypted or signed by users
* All server-to-server communications to be encrypted
* Allow anyone that would like to start their own network the tools to do so
* Do not rely on any public resources other than IP routing so it can be run on a censored Internet, or a closed LAN
* Allow server nodes to be run on any platform supported by Mono (Linux, OS X, BSD, and Windows, including x86, x86-64, ARM, s390, PowerPC)

## Future Goals
* Create a reputation system for users
* Create a standard API for 3rd party apps and command line users to:
  * Get and store data
  * Send and receive messages
  * Send broadcast (multiple receiver) messages
  * Push-style notifications of new messages
  * Get network user info as provided
* Allow browsers to participate in network using local storage
* Auto-push updates to all members

## Technology
CryptLink will use many existing technologies and ideas in new ways, including but not limited to:

* TLS Trust Networks with a private root Certificate Authority for servers
* HTTP(s) requests and sockets for general communication
* DNSSEC for public server name resolution and bootstrapping
* GPG/PGP for end-to-end message/data encryption and signing
* DHT (Distributed Hash Table) for resolving the host for message, data and user data
* Block chain for user reputation

## Licenses
See [Licenses.md](Licenses.md)