using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace CryptLink {
    public class CertificateSerializer : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            X509Certificate2 cert = (X509Certificate2)value;

            writer.WriteValue(
                Base64.EncodeBytes(
                    cert.Export(X509ContentType.Pkcs12)
                )
            );
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {

            if (reader?.Value != null && reader.Value.GetType() == typeof(string)) {
                
                X509Certificate2 cert = new X509Certificate2(
                    Base64.DecodeBytes(
                        (string)reader.Value
                    )
                );

                return cert;
            } else {
                return null;
            }
        }

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(X509Certificate2);
        }
    }
}
