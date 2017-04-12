using System;
using System.IO;
using NLog;
using Newtonsoft.Json;

namespace CryptLink {
	public class Config {

		[JsonIgnore]
		public bool ConfigIsNew { get; set; }
        [JsonIgnore]
        public string ErrorMessage { get; set; }
		private Logger logger { get; set; } = LogManager.GetCurrentClassLogger();
        private string ConfigPath { get; set; }

		public Config() { }

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
                    StreamReader file = File.OpenText(c.ConfigPath);
                    JsonSerializer serializer = new JsonSerializer();
                    return (Config)serializer.Deserialize(file, typeof(Config));
                } catch (Exception ex) {
                    c.logger.Error(ex, "Error loading config: " + c.ConfigPath);
                    c.ErrorMessage = ex.Message;
                    return c;
                }
            }
		}

        public void Save() {

            try {
                JsonSerializer serializer = new JsonSerializer();

                using (StreamWriter sw = new StreamWriter(ConfigPath)) {
                    using (JsonWriter writer = new JsonTextWriter(sw)) {
                        serializer.Serialize(writer, this);
                    }
                }
            } catch(Exception ex) {
                logger.Error(ex, "Error saving config: " + ConfigPath);
			}

}
	}
}