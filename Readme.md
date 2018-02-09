# CryptLink

A open source distributed network aims to provide:
* Distributed object storage
* An unlimited number of Immutable, mutable and rolling buffer blockchains in a swarm
* Communication via a number of standard and unusual protocols

## Possible Applications
* Host for large-sacle distributed databases and websites
* General communications network that provides security, identity and reputation enabling free-speech globally

## Status
CryptLink is not ready to run, see the [Roadmap.md](Roadmap.md)

## Contributions
Currently this project is written and maintained by one person, but I would welcome contributions from any skill level or background. If you don't know what to work on contact me.

## Primary Goals
* Provide a secure and reliable peer-to-peer-to-client messaging and data storage network
* Employ strong encryption end-to-end for all clients
* Employ strong peer to peer (server to server) encryption for all nodes
* Allow anyone that would like to start an independent network the tools to do so
* Do not rely on any public Internet resources such as existing Name Servers, DNS and SSL trust chains
* Allow peer (server) nodes to be run on any platform supported by Mono

## Secondary Goals
* Create a reputation system for network peers and users
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

## Licenses
See [Licenses.md](Licenses.md)