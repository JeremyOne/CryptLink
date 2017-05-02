using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq;
using ServiceStack;

namespace CryptLink.Services {
    //public class VersionResponse {
    //	public AppVersionInfo Result { get; set; }
    //}
	//https://github.com/lderache/ServiceStack-SelfHostDemo/blob/master/SelfHostRazorWebFormAuth/Program.cs

    [Route("/version")]
    public class VersionRequest { }

    [Route("/store")]
    public class StoreRequest {
        public TimeSpan ExpireAt { get; set; }
        public string SeralizedItem { get; set; }
    }

    public class StoreResponse {
        public Hash ItemHash { get; set; }
        public bool ItemAdded { get; set; }
    }

    [Route("/get")]
    public class GetRequest {
        public Hash ItemHash { get; set; }
    }

    public class ServiceHost : ServiceStack.Service {
        public Server Server { get; set; }

        public object Any(VersionRequest request) {
            CheckServerObject();

            if (Server?.ThisPeerInfo?.Version != null) {
				return Server.ThisPeerInfo.Version.ToString();
            } else {
                throw new NullReferenceException("Server version object is null, can't respond");
            }
        }

        public void CheckServerObject() {
            if (Server == null) {
                throw new NullReferenceException("Server object is null, can't respond");
            }
        }

        public object Any(StoreRequest request) {
            CheckServerObject();

            if (Server.AcceptingObjects) {
                //verify object
                if (request.SeralizedItem != null) {
                    var h = new HashableString(request.SeralizedItem, Server.Provider);
                    Server.ObjectCache.AddOrUpdate(h.Hash, h, request.ExpireAt);
                    return new StoreResponse() {
                        ItemHash = h.Hash,
                        ItemAdded = true
                    };
                }
            }

            return new StoreResponse() {
                ItemAdded = false
            };
        }

        public object Any(GetRequest request) {
            CheckServerObject();

            if (Server.HoldingObjects) {
                if (request.ItemHash == (Hash)null) {
                    throw new ArgumentException("ItemHash must be assigned");
                } else if (request.ItemHash.Valid() == false) {
                    throw new ArgumentException("ItemHash data is not valid");
                } else if (request.ItemHash.Provider != Server.Provider) {
                    throw new ArgumentException("ItemHash must be the provider type: " + Server.Provider.ToString());
                } else {
                    var cacheItem = Server.ObjectCache.Get(request.ItemHash);
                    return cacheItem.Value;
                }
            } else {
                throw new NotSupportedException("Server is not holding any objects.");
            }  
        }

    }

    public class ServiceHostBase : AppSelfHostBase {
        public ServiceHostBase() : base("HttpListener Self-Host", typeof(ServiceHost).Assembly) { }

        public override void Configure(Container container) {
            //container.Register<Server>(new Server());
            var server = new Server() {
                 Provider = Hash.HashProvider.SHA256
            };

            server.StartServices();

            container.Register(c => server).ReusedWithin(ReuseScope.Container);

        }
        
    }
}
