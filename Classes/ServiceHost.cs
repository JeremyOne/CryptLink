using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq;
using ServiceStack;

namespace CryptLink.Services {

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
        public ServiceConfig Config { get; set; }

        public object Any(VersionRequest request) {
            CheckServerObject();

            if (Config.ServerPeerInfo?.Version != null) {
				return Config.ServerPeerInfo.Version.ToString();
            } else {
                throw new NullReferenceException("Server version object is null, can't respond");
            }
        }

        public void CheckServerObject() {
            if (Config.Server == null) {
                throw new NullReferenceException("Server object is null, can't respond");
            }
        }

        public object Any(StoreRequest request) {
            CheckServerObject();

            if (Config.Server.AcceptingObjects) {
                //verify object
                if (request.SeralizedItem != null) {
                    var h = new HashableString(request.SeralizedItem, Config.Swarm.Provider);
                    Config.Server.ObjectCache.AddOrUpdate(h.Hash, h, request.ExpireAt);
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

            if (Config.Server.HoldingObjects) {
                if (request.ItemHash == (Hash)null) {
                    throw new ArgumentException("ItemHash must be assigned");
                } else if (request.ItemHash.Valid() == false) {
                    throw new ArgumentException("ItemHash data is not valid");
                } else if (request.ItemHash.Provider != Config.Swarm.Provider) {
                    throw new ArgumentException("ItemHash must be the provider type: " + Config.Swarm.Provider.ToString());
                } else {
                    var cacheItem = Config.Server.ObjectCache.Get(request.ItemHash);
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

        }
        
    }
}
