using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using NLog;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;

namespace CryptLink {
    
	public class Config {

		[JsonIgnore]
		public bool ConfigIsNew { get; set; }
        [JsonIgnore]
        public string Error { get; set; }

		private Logger logger { get; set; }
        private string ConfigPath { get; set; }

        [JsonIgnore]
		public X509Certificate2 ServerPrivateKey { get; set; }
        [JsonIgnore]
        public X509Certificate2 SwarmPrivateKey { get; set; }

        public byte[] SwarmPrivateExported { get; set; }
        public byte[] ServerPrivateExported { get; set; }

        public Peer PeerDetail { get; set; }
		public Swarm SwarmDetail { get; set; }

        public List<Peer> KnownPeers { get; set; }

        public IObjectCache DefaultCache { get; set; }

        public Config() {
			logger = LogManager.GetCurrentClassLogger();
		}

        public void SetDefaults(bool GenerateCerts, int DefaultCertLength, Hash.HashProvider Provider, string SwarmName, Uri SwarmPublicUri, Uri ServerUri) {

            logger.Debug("Setting default config");

            PeerDetail = new Peer() {
                KnownPublicUris = new System.Collections.Generic.List<Uri>(),
                LastKnownPublicUri = ServerUri,
                Provider = Provider,
                PubicallyAccessible = false,
                ConnectRetryMax = 10,
                Version = new AppVersionInfo() {
                    ApiCompartibilityVersion = new Version(1, 0, 0, 0),
                    ApiVersion = new Version(1, 0, 0, 0),
                    Name = Assembly.GetExecutingAssembly().GetName().FullName,
                    Version = Assembly.GetExecutingAssembly().GetName().Version
                }
            };

            SwarmDetail = new Swarm() {
                Accessibility = Swarm.JoinAccessibility.NoRestrictions,

                RootCertMinLength = DefaultCertLength,
                ServerCertMinLength = DefaultCertLength,
                UserCertMinLength = DefaultCertLength,

                BlobMinLength = 1024 * 128,
                BlobMaxLength = 1024 * 1024,
                BlobMinStorage = new TimeSpan(24,0,0),

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

            if (GenerateCerts) {
                GenerateAllCerts(true);
            }

        }

        public void GenerateAllCerts(bool Overwrite) {
            GenerateSwarmCerts(Overwrite);
            GenerateServerCerts(Overwrite);
        }

        public void GenerateSwarmCerts(bool Overwrite) {
            if (SwarmPrivateKey == null || Overwrite) {
                logger.Debug("Generating CA cert");

                var ca1 = new X509Certificate2Builder {
                    SubjectName = "CN=" + SwarmDetail.SwarmName,
                    KeyStrength = SwarmDetail.RootCertMinLength,
                    NotBefore = DateTime.Now,
                    NotAfter = DateTime.Now.AddYears(10)
                }.Build();

                logger.Debug("CA cert generated: ", ca1.Thumbprint);

                SwarmPrivateKey = ca1;
                SwarmDetail.SwarmCAPublicKey = Utility.GetPublicKey(ca1);
            }
        }

        public void GenerateServerCerts(bool Overwrite) {
            if (SwarmPrivateKey == null) {
                throw new NotImplementedException("Fetching cert from root not yet implemented.");
            } else if (ServerPrivateKey == null || Overwrite) {
                logger.Debug("Generating server cert");

                var server = new X509Certificate2Builder {
                    SubjectName = "CN=" + SwarmDetail.SwarmName,
                    KeyStrength = SwarmDetail.RootCertMinLength,
                    NotBefore = DateTime.Now,
                    NotAfter = DateTime.Now.AddYears(10)
                }.Build();

                logger.Debug("Server cert generated: ", server.Thumbprint);

                ServerPrivateKey = server;
                PeerDetail.PublicKey = Utility.GetPublicKey(server);
            }
        }

        public static Config Load(string _ConfigPath, bool CreateFile) {
            
            var c = new Config();
            c.ConfigPath = _ConfigPath;

            if (!System.IO.File.Exists(c.ConfigPath)) {
                if (CreateFile) {
                    c.ConfigIsNew = true;
                    c.Save();
                    return c;
                } else {
                    throw new FileNotFoundException("Could not load config file: " + c.ConfigPath);
                }
                     
            } else {
                c.ConfigIsNew = false;

                try {
                    //StreamReader file = File.OpenText(_ConfigPath);
                    //JsonSerializer serializer = new JsonSerializer();
                    //c = (Config)serializer.Deserialize(file, typeof(Config));

                    //file.Close();
                    //file.Dispose();

					c = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText(_ConfigPath));


                    if (c == null) {
                        throw new FileLoadException("Config file was not loaded: " + _ConfigPath);
                    }

                    if (c.SwarmPrivateExported != null) {
                        c.SwarmPrivateKey = new X509Certificate2(c.SwarmPrivateExported);
                    }

                    if (c.ServerPrivateExported != null) {
                        c.ServerPrivateKey = new X509Certificate2(c.ServerPrivateExported);
                    }

                    return c;
                } catch (Exception ex) {
                    c.logger.Error(ex, "Error loading config: " + c.ConfigPath);
                    c.Error = ex.Message;
                    return c;
                }
            }
		}

        public void Save() {

            try {
                JsonSerializer serializer = new JsonSerializer();

                using (StreamWriter sw = new StreamWriter(ConfigPath)) {
                    using (JsonWriter writer = new JsonTextWriter(sw)) {

                        if (SwarmPrivateKey != null) {
                            SwarmPrivateExported = SwarmPrivateKey.Export(X509ContentType.SerializedCert);
                        }
                        
                        if (ServerPrivateKey != null) {
                            ServerPrivateExported = ServerPrivateKey.Export(X509ContentType.SerializedCert);
                        }

                        serializer.Serialize(writer, this);
                    }
                }
            } catch(Exception ex) {
                logger.Error(ex, "Error saving config: " + ConfigPath);
			}

        }

    }
}