using System;
using System.IO;
using NLog;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CryptLink {
    
	public class ServiceConfig {
        [JsonIgnore]
        public bool ConfigIsNew { get; set; }

        [JsonIgnore]
        public string LoadError { get; set; }

        [JsonIgnore]
        public string ConfigPath { get; set; }

        public Server Server { get; set; }

        public Swarm Swarm { get; set; }

        public Peer ServerPeerInfo { get; set; }
        
        public static ServiceConfig Load(string _ConfigPath, bool CreateFile) {
            
            var c = new ServiceConfig();
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
                
                StreamReader file = File.OpenText(_ConfigPath);
                JsonSerializer serializer = new JsonSerializer();
                c = (ServiceConfig)serializer.Deserialize(file, typeof(ServiceConfig));

                file.Close();
                file.Dispose();

                if (c == null) {
                    throw new FileLoadException("Config file was not loaded: " + _ConfigPath);
                }

                return c;                
            }
		}

        public void Save() {

            if (ConfigPath == null) {
                throw new NullReferenceException("No path is set for this config instance, to save first call .SetPath()");
            }

            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(ConfigPath)) {
                using (JsonWriter writer = new JsonTextWriter(sw)) {
                    serializer.Serialize(writer, this);
                }
            }
        }

        public void SetPath(string Path) {
            ConfigPath = Path;
        }
    }
}