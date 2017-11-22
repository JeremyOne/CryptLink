# CryptLink FAQ
This document covers many facets of the CryptLink ideal, a evolving and 

## Tennents and Microfesto
Provide the best possible network that supports:
1. The greater good of humanity
2. Privacy, anonymity and free speech for people
3. Security, reputation and verification of identities


## What is a person
A walking, talking breathing human. Or anyone that can reason for itself with no outside influence, make effective decisions.

## What is an identity

## People
In a world where it is difficult to separate brands, corporations, governments and people, people must take the forefront, and lend their reputation as they see fit.

## Identities
In a world where groups (brands, corporations and governments) have power over people, the people must have power to choose the best option.

## Free Speech
CryptLink's main goal is to facilitate secure, anonymous free speech for anyone, anywhere as accessibly as possible. 

Also, provide a way for users reputation to be stored and anonymous anyone to encourage everyone to be an active and well intentioned member of the network.

### What about the "Bad Guys"?
Can these technologies be used for both "good" and "evil"? Yes. However, so can email, cell phones, pencil and paper, clay tablets and the spoken word. Unless every person on Earth is totally isolated and kept from all others, there is no way to stop knowledge and ideas from transferring from one person to another.

We strongly believe that unrestricted communication, especially people outside of your normal sphere of familiarity, is the best cure for ignorance. And ignorance breeds intolerance of outsiders in different political, social, racial, gender and ideological groups.

By fostering free speech and communication, we trust in the good of humanity. Everyone will have the chance to see and understand the lives of others that have a different way of living.

### Networks
Anyone can start their own instance of CryptLink on a public or private network. It should work equally as well on the unrestricted Internet as it does on a totally isolated LAN. The only basic requirement is access to an IP network.

### Peers
A peer is a server and client of a CryptLink network, it participates in exchanging messages and storing data. Typically runs on a system that is available most of the time on a IP address that changes infrequently. Peers provide API access to the network, but no user interface.

Peers are ultimately identified by their public key, and all data exchanged between peers is encrypted using their keys in a X509 trust structure.

For more on the trust network see the X509 section.

### Users
A user connects to a peer to use the network, but contributes only content to the network.

Users are ultimately identified by their public key, and all messages and data storage (they wish to attribute to themselves) is cryptographically signed, and optionally encrypted.

In a worst case scenario, if a network were disabled, or taken over by rogues, messages couldn't be altered, but could possibly be dropped.

### Block Chain

CryptLink uses a unique implementation of a block chain that is able to store any type of serializable data. A CryptLink network can contain any number of chains, and users can start their own chains to store and/or share any type of data.

Block chains data are secondary to the purpose of CryptLink (which is free communication), though they do facilitate storing data permanently.

### Chain Ownership

### Proof of Work

Traditional crypto currencies use a Proof of Work model that eat up computing resources and electricity on a ridiculous scale, resources that could serve much more mutual utility 

> "Proof of Work turned into a monster devouring electricity in the race for mining profit. Around 2012 the first serious complaints appeared, when the total performance of Bitcoin network surpassed that of the most productive supercomputer in the world"

*Excerpt from: [bytecoin.org](https://bytecoin.org/blog/proof-of-stake-proof-of-work-comparison/)*

CryptLink's implementation does not eat excessive resources generating blocks, and generation of blocks are not heavily incentivized. 

### Coins and Incentives

User: An 

Peers participating in a CryptLink network, or any particular chain does not provide any direct incentive, like mining bitcoins. However, users running a peer (or peers), get the indirect benefit of being able to store and retrieve data more quickly and easily through the merits of Peer and Chain Reputation.

### Chain Reputation
Participants in 

### Peer Reputation

### User Reputation

### Why Does Tracking Reputation Help the Network?
Why should a peer 

## Development

## DHT - Distributed Hash Table

### Unit Tests

We strongly believe that Unit Tests and Test Driven Design are powerful tools to keep large projects under control.

### HTTP and TCP vs UDP

Initial goals are to provide a HTTP(s) API, which are TCP based. That being said, UDP can be significantly faster and we will keep those options on the table, and hopefully implement UDP data transfer methods eventually.

### Why X509 Certificates?

### Why C#?
There are lots of good languages to accomplish what CryptLink is doing. C# is mature platform that likely will not be going anywhere soon.

* Mature, powerful and widely used
* Rich ecosystem of libraries via NuGet
* Many micro-service server libraries
* Strong type safety
* Runs well on lots of platforms (via Mono)
* Abundance of free and open source IDEs
  * Visual Studio Community 2015
  * MonoDevelop
  * Atom
  * (Many others)
* Any text editor and CLI tools:
  * MsBuild
  * Mono
  * DotNet Core
* No OS is significantly easier or harder to develop on
* Can be built and tested with 0% command line experience, or 100% through a CLI

Ultimately, the most valuable part of CryptLink are the protocols and technologies used in a way that people can trust (or not trust) anyone else. 

I fully expect 3rd parties to implement peers in many other languages with different benefits and trade-offs.