using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptLink {

    /// <summary>
    /// Peer - A network accessible host
    /// </summary>
    public class Peer : Hashable, IDisposable {
        public bool PubicallyAccessible { get; set; }
        public DateTime LastSeen { get; set; }
        public DateTime FirstSeen { get; set; }
        public Uri LastKnownPublicUri { get; set; }
        public List<Uri> KnownPublicIPs { get; set; }
        public List<Hash> KnownPeers { get; set; }
        public Hash ServerOperator { get; set; }
        public X509Certificate2 PublicKey { get; set; }
        public TimeSpan ConnectTimeOut { get; set; }
        public int ConnectRetryMax { get; set; }
        public int ConnectRetries { get; private set; }
        public AppVersionInfo Version { get; set; }

        //private static readonly ILog = LogManger.GetLogger(typeof(this));

        public override Hash.HashProvider Provider { get; set; }

        public ClientWebSocket ClientSocket { get; private set; }

        public override bool HashIsImmutable {
            get {
                return true;
            }
        }

        public override byte[] HashableData() {
            return PublicKey.PublicKey.EncodedKeyValue.RawData;
        }

        public bool TryConnect() {
            if (Connected()) {
                return true;
            } else {
                throw new NotImplementedException();
            }

            ConnectRetries = 0;

            if (ClientSocket == null) {
                //no client (yet)
                //var wscli = new ClientWebSocket();
                //var tokSrc = new CancellationTokenSource();
                //var task = wscli.ConnectAsync(LastKnownPublicUri, tokSrc.Token);

                //task.Wait();
                //task.Dispose();

                //Console.WriteLine($"WebSocket to {this.ToString()} {LastKnownPublicUri} OPEN!");
                //Console.WriteLine("SubProtocol: " + wscli.SubProtocol ?? "");

                //Console.WriteLine(@"Type ""exit<Enter>"" to quit... ");
                //for (var inp = Console.ReadLine(); inp != "exit"; inp = Console.ReadLine()) {
                //    task = wscli.SendAsync(
                //        new ArraySegment<byte>(Encoding.UTF8.GetBytes(inp)),
                //        WebSocketMessageType.Text,
                //        true,
                //        tokSrc.Token
                //    );

                //    task.Wait();
                //    task.Dispose();
                //    Console.WriteLine("**** sent msg");
                //}

                //if (wscli.State == WebSocketState.Open) {
                //    task = wscli.CloseAsync(WebSocketCloseStatus.NormalClosure, "", tokSrc.Token);
                //    task.Wait();
                //    task.Dispose();
                //}

                //tokSrc.Dispose();
                //Console.WriteLine("WebSocket CLOSED");

                
            }
        }

        public bool Connected() {
            return (ClientSocket?.State == WebSocketState.Open);
        }

        public void Dispose() {
            if (Connected()) {
                ClientSocket.Dispose();
            }
        }

    }
}
