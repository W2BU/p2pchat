using p2pchat.Common;
using System.Net.Sockets;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;

namespace p2pchat.ClientCore
{
    public class Client
    {
        public IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(Globals.SERVERADDRESS), Globals.PORT);

        private TcpClient tcpClient = new TcpClient();
        private UdpClient udpClient = new UdpClient();

        public ClientInfo localClientInfo = new ClientInfo();
        private List<ClientInfo> clients = new List<ClientInfo>();
        private List<Ack> ackResponses = new List<Ack>();

        private Thread tcpListenThread;
        private Thread udpListenThread;

        private ClientWindow window;

        private bool _isTcpListen = false;
        public bool isTcpListen
        {
            get { return _isTcpListen; }
            set
            {
                _isTcpListen = value;
                if (value)
                    ListenTcp();
            }
        }

        private bool _isUdpListen = false;
        public bool isUdpListen
        {
            get { return _isUdpListen; }
            set
            {
                _isUdpListen = value;
                if (value)
                    ListenUdp();
            }
        }

        public Client(ClientWindow form)
        {

            udpClient.AllowNatTraversal(true);
            udpClient.Client.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            localClientInfo.name = Environment.MachineName;
            localClientInfo.connectionType = ConnectionTypes.Unknown;
            localClientInfo.id = DateTime.Now.Ticks;

            window = form;

            var IPs = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            foreach (var IP in IPs)
                localClientInfo.internalAddresses.Add(IP.ToString());

        }

        public void UpdateServerAddress(string newAddress)
        {
            try
            {
                serverEndpoint.Address = IPAddress.Parse(newAddress);
            }
            catch (Exception ex)
            {
                window.OutToLog("Invalid Address. Default setting will be set.");
            }
        }

        public void UpdateUserName(string newName)
        {
            try
            {
                localClientInfo.name = newName;
            }
            catch (Exception ex)
            {
                window.OutToLog("Invalid name");
            }
        }


        public void SendMessageUdp(IPackaged item, IPEndPoint EP)
        {
            item.id = localClientInfo.id;

            byte[] data = Serializer.serializePackage(item);

            try
            {
                if (data != null)
                    udpClient.Send(data, data.Length, EP);
            }
            catch (Exception ex)
            {
                window.OutToLog("Error on UDP Send: " + ex.Message);
            }
        }

        public void SendMessageTcp(IPackaged package)
        {
            if (tcpClient.Connected)
            {
                byte[] Data = Serializer.serializePackage(package);

                try
                {
                    NetworkStream netStream = tcpClient.GetStream();
                    netStream.Write(Data, 0, Data.Length);
                }
                catch (Exception ex)
                {
                    window.OutToLog($"Error on TCP Send: {ex.Message}");
                }
            }
        }

        public void ConnectDisconnect()
        {
            if (tcpClient.Connected)
            {
                tcpClient.Client.Disconnect(true);

                isTcpListen = false;
                isUdpListen = false;
                clients.Clear();
                window.Disconnect(this);
                window.OutToLog("Disconnected");

            }
            else
            {
                try
                {
                    window.OutToLog("Connected with Internet access adapter");

                    tcpClient = new TcpClient();
                    tcpClient.Client.Connect(serverEndpoint);

                    isTcpListen = true;
                    isUdpListen = true;

                    SendMessageUdp(localClientInfo, serverEndpoint);
                    localClientInfo.internalEndPoint = udpClient.Client.LocalEndPoint.ToString();

                    Thread.Sleep(500);
                    SendMessageTcp(localClientInfo);

                    Thread keepAliveThread = new Thread(new ThreadStart(delegate
                    {
                        while (tcpClient.Connected)
                        {
                            Thread.Sleep(20000);
                            SendMessageTcp(new KeepAlive());
                        }
                    }));

                    keepAliveThread.IsBackground = true;
                    keepAliveThread.Start();

                }
                catch (Exception ex)
                {
                    window.OutToLog($"Error when connecting {ex.Message}");
                }
            }
        }

        private void ListenTcp()
        {
            tcpListenThread = new Thread(new ThreadStart(delegate
            {
                byte[] receivedBytes = new byte[Globals.BUFFERSIZE];
                string readString = "";
                NetworkStream clientStream = tcpClient.GetStream();

                while (isTcpListen)
                {
                    byte[] data = new byte[Globals.BUFFERSIZE];
                    try
                    {
                        StreamReader reader = new StreamReader(clientStream);
                        readString = reader.ReadLine();
                        if (readString == null)
                        {
                            break;
                        }
                        else
                        {
                            data = Encoding.UTF8.GetBytes(readString);
                            IPackaged package = Serializer.deserializePackage(data);
                            if (package != null) ProcessPackage(package);
                        }
                    }
                    catch (Exception ex)
                    {
                        window.OutToLog($"Error on TCP Send: {ex.Message}");
                    }
                }
            }));

            tcpListenThread.IsBackground = true;

            if (isTcpListen)
                tcpListenThread.Start();
        }

        private void ListenUdp()
        {
            udpListenThread = new Thread(new ThreadStart(delegate
            {
                while (isUdpListen)
                {
                    try
                    {
                        IPEndPoint endPoint = IPEndPoint.Parse(localClientInfo.internalEndPoint);

                        if (endPoint != null)
                        {
                            byte[] ReceivedBytes = udpClient.Receive(ref endPoint);
                            IPackaged package = Serializer.deserializePackage(ReceivedBytes);
                            ProcessPackage(package, endPoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        window.OutToLog($"Error on UDP Receive: {ex.Message}");
                    }
                }
            }));

            udpListenThread.IsBackground = true;

            if (isUdpListen)
                udpListenThread.Start();
        }

        private void ProcessPackage(IPackaged package, IPEndPoint endPoint = null)
        {
            if (package.typename == "Message")
            {
                Common.Message message = package as Common.Message;
                ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == package.id);

                if (message.id == 0)
                    window.OutToLog(message.from + ": " + message.content);
                if (message.id != 0 & endPoint != null & clientInfo != null)
                {
                    window.ReceiveDialogue(endPoint, new MessageReceivedEventArgs(clientInfo, message, endPoint));
                }

            }
            else if (package.typename == "ClientInfo")
            {
                ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == package.id);

                if (clientInfo == null)
                {
                    clients.Add(package as ClientInfo);
                    window.AddClient(package as ClientInfo);
                }
                else
                    clientInfo.Update(package as ClientInfo);
            }
            else if (package.typename == "Notification")
            {
                Notification N = package as Notification;

                if (N.Type == NotificationsTypes.Disconnected)
                {
                    ClientInfo CI = clients.FirstOrDefault(x => x.id == long.Parse(N.Tag.ToString()));

                    if (CI != null)
                    {
                        window.RemoveClient(CI);
                        clients.Remove(CI);
                    }
                }
                else if (N.Type == NotificationsTypes.ServerShutdown)
                {
                    window.OutToLog($"Server shutting down.");
                    tcpClient.Close();
                    isTcpListen = false;

                }
            }
            else if (package.typename == "Req")
            {
                Req R = (Req)package;

                ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == R.id);

                if (clientInfo != null)
                {
                    window.OutToLog($"Received Connection Request from: {clientInfo}");

                    IPEndPoint endPointResponded = FindReachableEndpoint(clientInfo);

                    if (endPointResponded != null)
                    {
                        window.OutToLog($"Connection Successfull to: {endPointResponded}");
                        window.InitiateDialogue(clientInfo, endPointResponded);
                    }
                }
            }
            else if (package.typename == "Ack")
            {
                Ack ack = package as Ack;

                if (ack.response)
                    ackResponses.Add(ack);
                else
                {
                    ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == ack.id);

                    var currentExternalEndPoint = IPEndPoint.Parse(clientInfo.externalEndPoint);

                    if (currentExternalEndPoint.Address.Equals(endPoint.Address) & currentExternalEndPoint.Port != endPoint.Port)
                    {
                        window.OutToLog($"Received Ack on Different Port ({endPoint.Port}). Updating ...");

                        currentExternalEndPoint.Port = endPoint.Port;
                    }

                    List<string> IPs = new List<string>();
                    clientInfo.internalAddresses.ForEach(new Action<string>(delegate (string IP) { IPs.Add(IP); }));

                    if (!clientInfo.externalEndPoint.Equals(endPoint.Address) & !IPs.Contains(endPoint.Address.ToString()))
                    {
                        window.OutToLog($"Received Ack on New Address ({endPoint.Address}). Updating ...");

                        clientInfo.internalAddresses.Add(endPoint.Address.ToString());
                    }

                    ack.response = true;
                    ack.recipientId = localClientInfo.id;
                    SendMessageUdp(ack, endPoint);
                }
            }
        }

        public void ConnectToClient(ClientInfo clientInfo)
        {
            Req R = new Req(localClientInfo.id, clientInfo.id);

            SendMessageTcp(R);

            window.OutToLog("Sent Connection Request To: " + clientInfo.ToString());

            Thread connect = new Thread(new ThreadStart(delegate
            {
                IPEndPoint responsiveEndPoint = FindReachableEndpoint(clientInfo);

                if (responsiveEndPoint != null)
                {
                    window.OutToLog("Connection Successfull to: " + responsiveEndPoint.ToString());
                    window.InitiateDialogue(clientInfo, responsiveEndPoint);
                }
            }));

            connect.IsBackground = true;
            connect.Start();
        }

        private IPEndPoint FindReachableEndpoint(ClientInfo clientInfo)
        {
            window.OutToLog("Attempting to Connect via LAN");

            for (int ip = 0; ip < clientInfo.internalAddresses.Count; ip++)
            {
                if (!tcpClient.Connected)
                    break;

                IPAddress IP = IPAddress.Parse(clientInfo.internalAddresses[ip]);

                IPEndPoint endPoint = new IPEndPoint(IP, IPEndPoint.Parse(clientInfo.internalEndPoint).Port);

                for (int i = 1; i < 4; i++)
                {
                    if (!tcpClient.Connected)
                        break;

                    window.OutToLog($"Sending Ack to {endPoint}. Attempt {i} of 3");

                    SendMessageUdp(new Ack(localClientInfo.id), endPoint);
                    Thread.Sleep(200);

                    Ack response = ackResponses.FirstOrDefault(a => a.recipientId == clientInfo.id);

                    if (response != null)
                    {
                        window.OutToLog($"Received Ack Responce from {endPoint}");

                        clientInfo.connectionType = ConnectionTypes.LAN;

                        ackResponses.Remove(response);

                        return endPoint;
                    }
                }
            }

            if (clientInfo.externalEndPoint != null)
            {
                window.OutToLog("Attempting to Connect via Internet");

                for (int i = 1; i < 100; i++)
                {
                    if (!tcpClient.Connected)
                        break;

                    window.OutToLog($"Sending Ack to {clientInfo.externalEndPoint}. Attempt {i} of 99");

                    SendMessageUdp(new Ack(localClientInfo.id), IPEndPoint.Parse(clientInfo.externalEndPoint));
                    Thread.Sleep(300);

                    Ack responce = ackResponses.FirstOrDefault(a => a.recipientId == clientInfo.id);

                    if (responce != null)
                    {
                        window.OutToLog($"Received Ack New from {clientInfo.externalEndPoint}");

                        clientInfo.connectionType = ConnectionTypes.WAN;

                        ackResponses.Remove(responce);

                        return IPEndPoint.Parse(clientInfo.externalEndPoint);
                    }
                }

                window.OutToLog($"Connection to {clientInfo.name} failed");
            }
            else
            {
                window.OutToLog("Client's External EndPoint is Unknown");
            }

            return null;
        }

        public class MessageReceivedEventArgs : EventArgs
        {
            public Common.Message message { get; set; }
            public ClientInfo clientInfo { get; set; }
            public IPEndPoint endPoint { get; set; }

            public MessageReceivedEventArgs(ClientInfo _clientInfo, Common.Message _message, IPEndPoint _endPoint)
            {
                clientInfo = _clientInfo;
                message = _message;
                endPoint = _endPoint;
            }
        }
    }
}
