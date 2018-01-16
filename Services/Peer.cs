using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using NLog;

namespace CryptLink {

    /// <summary>
    /// Peer - A network accessible host
    /// </summary>
    public class Peer : Hashable, IDisposable {
        public Hash ServerOperator { get; set; }
        public Cert Cert { get; set; }
        public AppVersionInfo Version { get; set; }
        public IServiceClient LocalClient { get; set; }
        public bool Initilized { get; private set; }

        public List<Hash> KnownPeers { get; set; } = new List<Hash>();
        public int Weight { get; set; } = 50;
        public List<IPeerTransport> Transports { get; set; } = new List<IPeerTransport>();
        public ConcurrentQueue<Hash> SendQueue { get; set; } = new ConcurrentQueue<Hash>();
        public TimeSpan ConnectTimeOut { get; set; } = new TimeSpan(0, 3, 0);

        /// <summary>
        /// Max number of threads to send in, a safe number is equal to the number of transports
        /// </summary>
        public int MaxSendingThreads { get; set; } = 2;

        IObjectCache sendCache;
        int sendingThreads = 0;

        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        public void Dispose() {
            foreach (var transport in Transports) {
                transport.Dispose();
            }
        }
        
        public T TryGet<T>(ComparableBytesAbstract Key) where T : Hashable {
            var transport = GetTransport();
            return transport.TryGet<T>(Key);
        }

        public void Initilize(IObjectCache SendCache) {
            if (Initilized) {
                throw new InvalidOperationException("This peer already initialized");
            }

            logger.Trace($"Peer '{this.ComputedHash}' initialized");
            sendCache = SendCache;
        }

        /// <summary>
        /// Enqueue a item to be sent, if there are threads available, a new sending thread will be started
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Item"></param>
        public void EnqueueSend<T>(T Item) where T : Hashable {
            if (!Initilized) {
                throw new InvalidOperationException("The peer has not been initialized");
            }

            sendCache.AddOrUpdate(Item.ComputedHash, Item, ConnectTimeOut);
            SendQueue.Enqueue(Item.ComputedHash);

            if (MaxSendingThreads < sendingThreads) {
                Task.Run(() => ProcessSendQueue());
            }
        }

        /// <summary>
        /// Processes the send queue
        /// </summary>
        /// <param name="ObjectCache"></param>
        public void ProcessSendQueue() {
            sendingThreads++;

            while (SendQueue.Count > 0) {
                Hash sendHash;
                SendQueue.TryDequeue(out sendHash);
                IPeerTransport transport = GetTransport();
                CacheItem sendItem = sendCache.Get(sendHash);

                if (sendItem == null) {
                    logger.Warn("A peer send item expired, it will not be resent");
                } else {
                    if (transport.TryPut(sendItem)) {
                        //successful send, remove cached item
                        sendCache.Remove(sendItem.Key);
                    }

                    sendCache.Expire(DateTime.Now - ConnectTimeOut, true);
                }
            }

            sendingThreads--;
        }

        /// <summary>
        /// Gets the highest weighted available transport for this peer
        /// </summary>
        public IPeerTransport GetTransport() {
            if (Transports == null || Transports.Count == 0) {
                throw new NullReferenceException("No transports on peer: " + this.ComputedHash.ToString());
            } else if (Transports.Count == 1) {
                return Transports.First();
            } else {
                return (from t in Transports where t.Ready orderby t.Weight select t).FirstOrDefault();
            }
        }

        public override byte[] GetHashableData() {
            return Cert.GetHashableData();
        }
        
    }
}
