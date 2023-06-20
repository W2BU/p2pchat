using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace p2pchat.Common
{
    static class Serializer
    {
        public static byte[] serializePackage(IPackaged package)
        {
            var packageDataString = JsonConvert.SerializeObject(package);
            byte[] data = Encoding.Unicode.GetBytes(packageDataString);
            return data;    
        }

        public static IPackaged deserializePackage(byte[] bytes)
        {
            var decodedDataString = Encoding.Unicode.GetString(bytes);
            var parsedObject = JObject.Parse(decodedDataString);
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
            }
            return null;
        }
    }
}

