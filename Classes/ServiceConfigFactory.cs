using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    public class ServiceConfigFactory {

        public static ServiceConfig Defaults(bool GenerateCerts, int DefaultCertLength, Hash.HashProvider Provider, 
            string SwarmName, Uri SwarmPublicUri, Uri ServerUri, Uri LocalServerUri,
            X509Certificate2 SigningCert, Version ServiceApiCompartibilityVersion, Version ServiceApiVersion) {

            var config = new ServiceConfig();

            config.ServerPeerInfo = new Peer() {
                KnownPublicUris = new List<Uri>(),
                LastKnownPublicUri = ServerUri,
                Provider = Provider,
                PubicallyAccessible = false,
                ConnectRetryMax = 10,
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
                 
            };
            
            if (GenerateCerts) {
                var swarmKey = new X509Certificate2Builder {
                    Issuer = SigningCert,
                    SubjectName = "CN=" + config.Swarm.SwarmName,
                    KeyStrength = config.Swarm.RootCertMinLength,
                    NotBefore = DateTime.Now,
                    NotAfter = DateTime.Now.AddYears(10)
                }.Build();
                
                config.Swarm.PrivateKey = swarmKey;
                //config.Swarm.PublicKey = Utility.GetPublicKey(swarmKey);

                var serverKey = new X509Certificate2Builder {
                    Issuer = SigningCert,
                    SubjectName = "CN=" + config.Swarm.SwarmName,
                    KeyStrength = config.Swarm.RootCertMinLength,
                    NotBefore = DateTime.Now,
                    NotAfter = DateTime.Now.AddYears(10)
                }.Build();

                config.Server.PrivateKey = serverKey;
                config.ServerPeerInfo.PublicKey = Utility.GetPublicKey(serverKey);

            }
            
            return config;

        }
        

    }
}
