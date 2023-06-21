using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace p2pchat.Common
{
    static class Serializer
    {
        public static byte[] serializePackage(IPackaged package)
        {
            string packageDataString = JsonConvert.SerializeObject(package);
            byte[] data = Encoding.UTF8.GetBytes(packageDataString + "\r\n"); // rn to separate messages
            return data;    
        }

        public static IPackaged deserializePackage(byte[] bytes)
        {
            string decodedDataString = Encoding.UTF8.GetString(bytes);
            JObject parsedObject = JObject.Parse(decodedDataString);
            string typeOfPackage = parsedObject["typename"].ToString().ToUpper();
            switch (typeOfPackage)
            {
                case "CLIENTINFO":
                    return JsonConvert.DeserializeObject<ClientInfo>(decodedDataString);
                case "MESSAGE":
                    return JsonConvert.DeserializeObject<Message>(decodedDataString);
                case "ACK":
                    return JsonConvert.DeserializeObject<Ack>(decodedDataString);
                case "REQ":
                    return JsonConvert.DeserializeObject<Req>(decodedDataString);
                case "KEEPALIVE":
                    return JsonConvert.DeserializeObject<KeepAlive>(decodedDataString);
                case "NOTIFICATION":
                    return JsonConvert.DeserializeObject<Notification>(decodedDataString);
            }
            return null;
        }
    }
}

