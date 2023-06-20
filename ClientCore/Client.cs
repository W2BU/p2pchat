using p2pchat.Common;
using System.Net.Sockets;
using System.Net;

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
                window.outToLog("Invalid Address. Default setting will be set.");
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
                window.outToLog("Invalid name");
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
                window.outToLog("Error on UDP Send: " + ex.Message);
            }
        }

        public void SendMessageTcp(IPackaged package)
        {
            if (tcpClient.Connected)
            {
                byte[] Data = Serializer.serializePackage(package);

                try
                {
                    NetworkStream NetStream = tcpClient.GetStream();
                    NetStream.Write(Data, 0, Data.Length);
                }
                catch (Exception ex)
                {
                    window.outToLog($"Error on TCP Send: {ex.Message}");
                }
            }
        }

        public void Connect()
        {
            if (tcpClient.Connected)
            {
                tcpClient.Client.Disconnect(true);

                isTcpListen = false;
                isUdpListen = false;
                clients.Clear();
            }
            else
            {
                try
                {
                    window.outToLog("Connected with Internet access adapter");

                    tcpClient = new TcpClient();
                    tcpClient.Client.Connect(serverEndpoint);

                    isTcpListen = true;
                    isUdpListen = true;

                    SendMessageUdp(localClientInfo, serverEndpoint);
                    localClientInfo.internalEndPoint = udpClient.Client.LocalEndPoint.ToString();

                    Thread.Sleep(1000);
                    SendMessageTcp(localClientInfo);

                    /*Thread keepAliveThread = new Thread(new ThreadStart(delegate
                    {
                        while (tcpClient.Connected)
                        {
                            Thread.Sleep(10000);
                            SendMessageTcp(new KeepAlive());
                        }
                    }));

                    keepAliveThread.IsBackground = true;
                    keepAliveThread.Start();*/

                }
                catch (Exception ex)
                {
                    window.outToLog($"Error when connecting {ex.Message}");
                }
            }
        }

        private void ListenTcp()
        {
            tcpListenThread = new Thread(new ThreadStart(delegate
            {
                byte[] receivedBytes = new byte[4096];
                int BytesRead = 0;

                while (isTcpListen)
                {
                    try
                    {
                        BytesRead = tcpClient.GetStream().Read(receivedBytes, 0, receivedBytes.Length);

                        if (BytesRead == 0)
                            break;
                        else
                        {
                            IPackaged package = Serializer.deserializePackage(receivedBytes);
                            ProcessItem(package);
                        }
                    }
                    catch (Exception ex)
                    {
                        window.outToLog($"Error on TCP Send: {ex.Message}");
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
                            ProcessItem(package, endPoint);
                        }
                    }
                    catch (Exception ex)
                    {
                        window.outToLog($"Error on UDP Receive: {ex.Message}");
                    }
                }
            }));

            udpListenThread.IsBackground = true;

            if (isUdpListen)
                udpListenThread.Start();
        }

        private void ProcessItem(IPackaged package, IPEndPoint endPoint = null)
        {
            if (package.typename == "Message")
            {
                Common.Message message = package as Common.Message;
                ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == package.id);

                if (message.id == 0)
                    window.outToLog(message.from + ": " + message.content);

            }
            else if (package.typename == "ClientInfo")
            {
                ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == package.id);

                if (clientInfo == null)
                {
                    clients.Add(package as ClientInfo);
                    window.addClient(package as ClientInfo);
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
                        window.removeClient(CI);
                        clients.Remove(CI);
                    }
                }
                else if (N.Type == NotificationsTypes.ServerShutdown)
                {
                    window.outToLog($"Server shutting down.");
                    Connect();
                }
            }
            else if (package.typename == "Req")
            {
                Req R = (Req)package;

                ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == R.id);

                if (clientInfo != null)
                {
                    window.outToLog($"Received Connection Request from: {clientInfo}");

                    IPEndPoint endPointResponded = FindReachableEndpoint(clientInfo);

                    if (endPointResponded != null)
                    {
                        window.outToLog($"Connection Successfull to: {endPointResponded}");
                    }
                }
            }
            else if (package.typename == "Ack")
            {
                Ack A = package as Ack;

                if (A.response)
                    ackResponses.Add(A);
                else
                {
                    ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == A.id);

                    var currentExternalEndPoint = IPEndPoint.Parse(clientInfo.externalEndPoint);

                    if (currentExternalEndPoint.Address.Equals(endPoint.Address) & currentExternalEndPoint.Port != endPoint.Port)
                    {
                        window.outToLog($"Received Ack on Different Port ({endPoint.Port}). Updating ...");

                        currentExternalEndPoint.Port = endPoint.Port;
                    }

                    List<string> IPs = new List<string>();
                    clientInfo.internalAddresses.ForEach(new Action<string>(delegate (string IP) { IPs.Add(IP); }));

                    if (!clientInfo.externalEndPoint.Equals(endPoint.Address) & !IPs.Contains(endPoint.Address.ToString()))
                    {
                        window.outToLog($"Received Ack on New Address ({endPoint.Address}). Updating ...");

                        clientInfo.internalAddresses.Add(endPoint.Address.ToString());
                    }

                    A.response = true;
                    A.recipientId = localClientInfo.id;
                    SendMessageUdp(A, endPoint);
                }
            }
        }

        public void ConnectToClient(ClientInfo clientInfo)
        {
            Req R = new Req(localClientInfo.id, clientInfo.id);

            SendMessageTcp(R);

            window.outToLog("Sent Connection Request To: " + clientInfo.ToString());

            Thread connect = new Thread(new ThreadStart(delegate
            {
                IPEndPoint responsiveEndPoint = FindReachableEndpoint(clientInfo);

                if (responsiveEndPoint != null)
                {
                    window.outToLog("Connection Successfull to: " + responsiveEndPoint.ToString());
                }
            }));

            connect.IsBackground = true;

            connect.Start();
        }

        private IPEndPoint FindReachableEndpoint(ClientInfo clientInfo)
        {
            window.outToLog("Attempting to Connect via LAN");

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

                    window.outToLog($"Sending Ack to {endPoint}. Attempt {i} of 3");

                    SendMessageUdp(new Ack(localClientInfo.id), endPoint);
                    Thread.Sleep(200);

                    Ack responce = ackResponses.FirstOrDefault(a => a.recipientId == clientInfo.id);

                    if (responce != null)
                    {
                        window.outToLog($"Received Ack Responce from {endPoint.ToString()}");

                        clientInfo.connectionType = ConnectionTypes.LAN;

                        ackResponses.Remove(responce);

                        return endPoint;
                    }
                }
            }

            if (clientInfo.externalEndPoint != null)
            {
                window.outToLog("Attempting to Connect via Internet");

                for (int i = 1; i < 100; i++)
                {
                    if (!tcpClient.Connected)
                        break;

                    window.outToLog($"Sending Ack to {clientInfo.externalEndPoint}. Attempt {i} of 99");

                    SendMessageUdp(new Ack(localClientInfo.id), IPEndPoint.Parse(clientInfo.externalEndPoint));
                    Thread.Sleep(300);

                    Ack responce = ackResponses.FirstOrDefault(a => a.recipientId == clientInfo.id);

                    if (responce != null)
                    {
                        window.outToLog($"Received Ack New from {clientInfo.externalEndPoint}");

                        clientInfo.connectionType = ConnectionTypes.WAN;

                        ackResponses.Remove(responce);

                        return IPEndPoint.Parse(clientInfo.externalEndPoint);
                    }
                }

                window.outToLog($"Connection to {clientInfo.name} failed");
            }
            else
            {
                window.outToLog("Client's External EndPoint is Unknown");
            }

            return null;
        }

        public class MessageReceivedEventArgs : EventArgs
        {
            public Common.Message message { get; set; }
            public ClientInfo clientInfo { get; set; }
            public IPEndPoint EstablishedEP { get; set; }

            public MessageReceivedEventArgs(ClientInfo _clientInfo, Common.Message _message, IPEndPoint _establishedEP)
            {
                clientInfo = _clientInfo;
                message = _message;
                EstablishedEP = _establishedEP;
            }
        }
    }
}
