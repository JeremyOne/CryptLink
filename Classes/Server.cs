//using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Threading;
using NLog;

namespace CryptLink {

    /// <summary>
    /// A server that runs locally, processes requests, caches data
    /// </summary>
    public class Server : IDisposable {
        public Hash.HashProvider Provider { get; private set; }
        public X509Certificate2 ServerCert { get; private set; }
        public IObjectCache ObjectCache { get; private set; }
        public long SocketMessageCount { get; private set; }
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public bool LogToConsole { get; set; } = true;

        /// <summary>
        /// All known servers
        /// </summary>
        private ConsistentHash<Peer> KnownPeers;

        /// <summary>
        /// Objects for all chains this server is participating in
        /// </summary>
        //private Dictionary<Hash, BlockChain> ParticipatingChains = new Dictionary<Hash, BlockChain>();

        //private Dictionary<Hash, User> KnownUsers = new Dictionary<Hash, User>();

        public Server(Hash.HashProvider _Provider, X509Certificate2 _ServerCert, IObjectCache _ObjectCache, string SocketUrl) {

        //    logger.Info(this.ToString(), "Starting...");

        //    Provider = _Provider;
        //    ServerCert = _ServerCert;
        //    ObjectCache = _ObjectCache;

        //    KnownPeers = new ConsistentHash<Peer>(Provider);

        //    SocketServer = new WebSocketServer(SocketUrl);

        //    if (_ServerCert != null) {
        //        SocketServer.Certificate = ServerCert;
        //    }           

        //    SocketServer.RestartAfterListenError = true;
        //    SocketServer.Start(socket =>
        //    {
        //        socket.OnOpen = () => SocketOpen();
        //        socket.OnClose = () => SocketClose();
        //        socket.OnMessage = message => SocketMessage(message);
        //        socket.OnError = error => SocketError(error);
        //        socket.OnPing = ping => SocketPing(ping);
        //        socket.OnPong = pong => SocketPong(pong);
        //        socket.OnBinary = binary => SocketBinary(binary);
        //        SocketConnection = socket;
        //    });    

        }

        //public WebSocketServer SocketServer;



        private void SocketOpen() {
            logger.Info(this.ToString() + " Socket opened");
        }

        private void SocketClose() {
            logger.Info(this.ToString() + " Socket closed");
        }

        private void SocketError(Exception Ex) {
            logger.Error(Ex, this.ToString());
        }

        private void SocketMessage(string Message) {
            SocketMessageCount++;
            logger.Info(this.ToString() + " Message: " + Message);
        }

        private void SocketBinary(byte[] Message) {
            SocketMessageCount++;
            logger.Info(this.ToString() + " Binary: " + Base64.EncodeBytes(Message));
        }

        private void SocketPing(byte[] Ping) {
            logger.Info(this.ToString() + " Ping: " + Base64.EncodeBytes(Ping));
        }

        private void SocketPong(byte[] Pong) {
            logger.Info(this.ToString() + " Pong: " + Base64.EncodeBytes(Pong));
        }

        public void Dispose() {
            logger.Info(this.ToString() + " Shutdown...");
            ObjectCache.Dispose();
            //SocketServer.Dispose();

            foreach (var client in KnownPeers.AllNodes) {
                client.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        public T Get<T>(CByte Key) where T : Hashable {

            if (ObjectCache.Exists(Key)) {
                return ObjectCache.Get<T>(Key);
            } else {
                throw new NotImplementedException();
            }
        }

        public Peer GetPeer(CByte Key) {
            throw new NotImplementedException();
        }
    }

}
