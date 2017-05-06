using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptLink {
    public static class ConfigStatic {
        public static string DefaultConfig { get; set; } = "config.json";
        static Logger logger { get; set; } = LogManager.GetCurrentClassLogger();

        public static Config _config;
        public static Config Config {
            get {
                if (!Loaded) {
                    Load();
                }
                return _config;
            }

            set {
                _config = value;
            }
        }

        public static bool Loaded {
            get{
                return _config != null;
            }
        }

        public static void Load() {
            Load(DefaultConfig);
        }

        /// <summary>
        /// Clears the memory config and deletes the file if it exists
        /// </summary>
        public static void Delete() {
            _config = null;

            if (System.IO.File.Exists(DefaultConfig)) {
                try {
                    System.IO.File.Delete(DefaultConfig);
                } catch (Exception ex) {
                    logger.Log(LogLevel.Error, ex, "Error deleting StaticConfig");
                }
            }
        }

        public static void Load(string Path) {
            _config = CryptLink.Config.Load(Path, true);
        }
    }
}
