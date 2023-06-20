using System.Net.Sockets;
using System.Reflection;
using Newtonsoft.Json;

namespace p2pchat.Common
{
    public enum ConnectionTypes
    {
        Unknown,
        LAN,
        WAN
    }

    public enum NotificationsTypes { ServerShutdown, Disconnected }

    public class Notification : IPackaged
    {
        public long id { get; set; }
        public string typename
        {
            get { return GetType().Name; }
            set { }
        }

        public NotificationsTypes Type { get; set; }
        public object Tag { get; set; }

        public Notification(NotificationsTypes _Type, object _Tag)
        {
            Type = _Type;
            Tag = _Tag;
        }
    }

    public class ClientInfo : IPackaged
    {
        public string name { get; set; }
        public long id { get; set; }
        public string externalEndPoint { get; set; }
        public string internalEndPoint { get; set; }
        public string typename
        {
            get { return GetType().Name; }
            set { }
        }

        public List<string> internalAddresses = new List<string>();
        public ConnectionTypes connectionType { get; set; }

        [JsonIgnore] //server use only
        public TcpClient client;

        [JsonIgnore] //server use only
        public bool initialized;

        public bool Update(ClientInfo clientInfo)
        {
            if (id == clientInfo.id)
            {
                foreach (PropertyInfo P in clientInfo.GetType().GetProperties())
                    if (P.GetValue(clientInfo) != null)
                        P.SetValue(this, P.GetValue(clientInfo));

                if (clientInfo.internalAddresses.Count > 0)
                {
                    internalAddresses.Clear();
                    internalAddresses.AddRange(clientInfo.internalAddresses);
                }
            }

            return (id == clientInfo.id);
        }
    }

    public class Message : IPackaged
    {
        public string from { get; set; }
        public string to { get; set; }
        public string content { get; set; }
        public long id { get; set; }
        public long recipientId { get; set; }

        public string typename
        {
            get { return GetType().Name; }
            set { }
        }

        public Message(string from, string to, string content)
        {
            this.from = from;
            this.to = to;
            this.content = content;
        }
    }

    public class KeepAlive : IPackaged
    {
        public long id { get; set; }
        public string typename
        {
            get { return GetType().Name; }
            set { }
        }
    }

    public class Req : IPackaged
    {
        public long id { get; set; }
        public long recipientId { get; set; }

        public string typename
        {
            get { return GetType().Name; }
            set { }
        }
        public Req(long Sender_ID, long Recipient_ID)
        {
            id = Sender_ID;
            recipientId = Recipient_ID;
        }
    }

    public class Ack : IPackaged
    {
        public long id { get; set; }
        public long recipientId { get; set; }
        public bool response { get; set; }

        public string typename
        {
            get { return GetType().Name; }
            set { }
        }

        public Ack(long senderId)
        { 
            id = senderId;
        }
    }
}
