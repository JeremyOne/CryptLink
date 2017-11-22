﻿using System;
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
        public ServiceConfig SConfig { get; set; }

        public ServiceHost() { }

        public object Any(VersionRequest request) {
            CheckServerObject();

            if (SConfig?.ServerPeerInfo?.Version != null) {
				return SConfig.ServerPeerInfo.Version.ApiVersion.ToString();
            } else {
                throw new NullReferenceException("Server version object is null, can't respond");
            }
        }

        public void CheckServerObject() {
            if (SConfig?.Server == null) {
                throw new NullReferenceException("ServiceHost.SConfig.Server is null, can't respond");
            } else if (SConfig?.Server?.ObjectCache == null) {
                throw new NullReferenceException("ServiceHost.SConfig.Server.ObjectCache is null, can't respond");
            }
        }

        public object Any(StoreRequest request) {
            CheckServerObject();

            if (SConfig.Server.AcceptingObjects) {
                //verify object
                if (request.SeralizedItem != null) {
                    var h = new HashableString(request.SeralizedItem, SConfig.Swarm.Provider);
                    SConfig.Server.ObjectCache.AddOrUpdate(h.Hash, h, request.ExpireAt);
                    return new StoreResponse() {
                        ItemHash = h.Hash,
                        ItemAdded = true
                    };
                }
            }

            //didn't add the item
            return new StoreResponse() {
                ItemAdded = false
            };
        }

        public object Any(GetRequest request) {
            CheckServerObject();

            if (SConfig.Server.HoldingObjects) {
                if (request.ItemHash == (Hash)null) {
                    throw new ArgumentException("ItemHash must be assigned");
                } else if (request.ItemHash.Valid() == false) {
                    throw new ArgumentException("ItemHash data is not valid");
                } else if (request.ItemHash.Provider != SConfig.Swarm.Provider) {
                    throw new ArgumentException("ItemHash must be the provider type: " + SConfig.Swarm.Provider.ToString());
                } else {
                    var cacheItem = SConfig.Server.ObjectCache.Get(request.ItemHash);
                    return cacheItem.Value;
                }
            } else {
                throw new NotSupportedException("Server is not holding any objects.");
            }  
        }

    }

    public class ServiceHostBase : AppSelfHostBase {
        public ServiceHostBase() : base("HttpListener Self-Host", typeof(ServiceHost).Assembly) { }

        public override void Configure(Container container) { }
        
    }
}
