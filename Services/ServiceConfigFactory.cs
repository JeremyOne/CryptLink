using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    public class ServiceConfigFactory {

        public static ServiceConfig TestDefaults() {
            return ServiceConfigFactory.Defaults(
                true,
                1024,
                Hash.HashProvider.MD5,
                "test",
                new Uri("http://127.0.0.1:54321"),
                new Uri("http://127.0.0.1:54321"),
                new Uri("http://127.0.0.1:54321"),
                null,
                new Version(1, 0, 0, 0),
                new Version(1, 0, 0, 0),
                new DictionaryCache() {
                    AcceptingObjects = true,
                    ManageEvery = new TimeSpan(0, 0, 0),
                    ManageEveryIOCount = 0,
                    MaxCollectionCount = 10000000,
                    MaxCollectionSize = 1024 * 1024 * 1024,
                    MaxExpiration = new TimeSpan(0, 0, 20),
                    MaxObjectSize = 1024 * 1024
                }
            );
        }

        public static ServiceConfig Defaults(bool GenerateCerts, int DefaultCertLength, Hash.HashProvider Provider, 
            string SwarmName, Uri SwarmPublicUri, Uri ServerUri, Uri LocalServerUri,
            X509Certificate2 SigningCert, Version ServiceApiCompartibilityVersion, Version ServiceApiVersion, IObjectCache ObjectCache) {

            var config = new ServiceConfig();

            config.ServerPeerInfo = new Peer() {
                Provider = Provider,
                Version = new AppVersionInfo() {
                    ApiCompartibilityVersion = ServiceApiCompartibilityVersion,
                    ApiVersion = ServiceApiVersion,
                    Name = Assembly.GetExecutingAssembly().GetName().FullName,
                    Version = Assembly.GetExecutingAssembly().GetName().Version
                }
            };

            config.Swarm = new Swarm() {
                Accessibility = Swarm.JoinAccessibility.NoRestrictions,

                RootCertMinLength = DefaultCertLength,
                ServerCertMinLength = DefaultCertLength,
                UserCertMinLength = DefaultCertLength,

                BlobMinLength = 1024 * 128,
                BlobMaxLength = 1024 * 1024,
                BlobMinStorage = new TimeSpan(24, 0, 0),

                ItemMinLength = 1024,
                ItemMaxLength = 1024 * 128,
                ItemMinStorage = new TimeSpan(24, 0, 0),

                MessageMinLength = 1024,
                MessageMaxLength = 1024 * 128,
                MessageMinStorage = new TimeSpan(4, 0, 0),

                PaddingEnforced = true,
                SwarmName = SwarmName,
                PublicAddress = SwarmPublicUri
            };

            config.Server = new Server() {
                 ServiceAddress = LocalServerUri.OriginalString,
                 StoreCache = ObjectCache
            };

            //start the cache
            config.Server.StoreCache.Initialize();
           
            if (GenerateCerts) {
                var swarmKey = new X509Certificate2Builder {
                    Issuer = SigningCert,
                    SubjectName = "CN=" + config.Swarm.SwarmName,
                    KeyStrength = config.Swarm.RootCertMinLength,
                    NotBefore = DateTime.Now,
                    NotAfter = DateTime.Now.AddYears(10)
                }.Build();
                
                config.Swarm.CertManager = new CertificateManager(swarmKey);
                //config.Swarm.PublicKey = Utility.GetPublicKey(swarmKey);

                var serverKey = new X509Certificate2Builder {
                    Issuer = SigningCert,
                    SubjectName = "CN=" + config.Swarm.SwarmName,
                    KeyStrength = config.Swarm.RootCertMinLength,
                    NotBefore = DateTime.Now,
                    NotAfter = DateTime.Now.AddYears(10)
                }.Build();

                config.Server.CertManager = new CertificateManager(serverKey);
                config.ServerPeerInfo.Cert = Utility.GetPublicKey(serverKey);

            }
            
            return config;

        }
        

    }
}
