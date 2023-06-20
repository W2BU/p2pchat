
using p2pchat.Common;
using System.Net;
using System.Net.Sockets;

namespace p2pchat.ServerCore
{

    public class Server
    {
        static private List<ClientInfo> clients = new List<ClientInfo>();

        static private IPEndPoint tcpEndpoint = new IPEndPoint(IPAddress.Any, Globals.PORT);
        static private TcpListener tcp = new TcpListener(tcpEndpoint);

        static private IPEndPoint udpEndpoint = new IPEndPoint(IPAddress.Any, Globals.PORT);
        static private  UdpClient udp = new UdpClient(udpEndpoint);

        private ServerWindow window;

        public Server(ServerWindow form)
        {
            window = form;
            window.outToLog("Server started");

            LogHostIp();

            Thread tcpThread = new Thread(new ThreadStart(TcpListen));
            Thread udpThread = new Thread(new ThreadStart(UdpListen));

            tcpThread.Start();
            udpThread.Start();
        }

        public void Stop()
        {
            window.outToLog("Shutting down...");
            BroadcastTCP(new Notification(NotificationsTypes.ServerShutdown, null));
            tcp.Stop();
        }

        private void TcpListen()
        {
            tcp.Start();
            window.outToLog("Tcp listener started");
            while (true)
            {
                try
                {
                    TcpClient newClient = tcp.AcceptTcpClient();
                    Action<object> processClient = new Action<object>(delegate (object _client)
                    {
                        TcpClient client = _client as TcpClient;
                        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                        byte[] data = new byte[4096];
                        int bytesCount = 0;
                        while (client.Connected)
                        {
                            try
                            {
                                bytesCount = client.GetStream().Read(data, 0, data.Length);
                            }
                            catch
                            {
                                DisconnectClient(client);
                            }

                            if (bytesCount == 0)
                                break;
                            else if (client.Connected)
                            {
                                IPackaged package = Serializer.deserializePackage(data);
                                ProcessPackage(package, ProtocolType.Tcp, null, client);
                            }
                        }

                        DisconnectClient(client);
                    });

                    Thread ThreadProcessData = new Thread(new ParameterizedThreadStart(processClient));
                    ThreadProcessData.Start(newClient);
                }
                catch (Exception e)
                {
                    window.outToLog($"TCP Error: {e.Message}");
                }
            }
        }

        private void UdpListen()
        {
            window.outToLog("UDP Listener Started");

            while (true)
            {
                byte[] receivedBytes = null;

                try
                {
                    receivedBytes = udp.Receive(ref udpEndpoint);
                }
                catch (Exception ex)
                {
                    window.outToLog($"UDP Error: {ex.Message}");
                }

                if (receivedBytes != null)
                {
                    IPackaged Item = Serializer.deserializePackage(receivedBytes);
                    ProcessPackage(Item, ProtocolType.Udp, udpEndpoint);
                }
            }
        }


        private void DisconnectClient(TcpClient client)
        {
            ClientInfo clientInfo = clients.FirstOrDefault(x => x.client == client);
            if (clientInfo != null)
            {
                window.outToLog($"Client Disconnected {clientInfo.client.Client.RemoteEndPoint}");
                clients.Remove(clientInfo);
                client.Close();

                BroadcastTCP(new Notification(NotificationsTypes.Disconnected, clientInfo.id));
            }
        }

        private void ProcessPackage (
            IPackaged package,
            ProtocolType protocol,
            IPEndPoint endPoint = null,
            TcpClient client = null
        )
        {
            IPackaged debug = package;
            if (package.typename == "ClientInfo")
            {
                ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == ((ClientInfo)package).id);

                if (clientInfo == null)
                {
                    clientInfo = package as ClientInfo;
                    clients.Add(clientInfo);

                    if (endPoint != null)
                        window.outToLog($"Client Added: UDP EP: {endPoint.Address}:{endPoint.Port}, Name: {clientInfo.name}");
                    else if (client != null)
                        window.outToLog($"Client Added: TCP EP: {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}, Name: {clientInfo.name}");
                }
                else
                {
                    clientInfo.Update(package as ClientInfo);

                    if (endPoint != null)
                        window.outToLog($"Client Updated: UDP EP: {endPoint.Address}:{endPoint.Port}, Name: {clientInfo.name}");
                    else if (client != null)
                        window.outToLog($"Client Updated: TCP EP: {((IPEndPoint)client.Client.RemoteEndPoint).Address}:{((IPEndPoint)client.Client.RemoteEndPoint).Port}, Name: {clientInfo.name}");
                }

                if (endPoint != null)
                    clientInfo.externalEndPoint = endPoint.ToString();

                if (client != null)
                    clientInfo.client = client;

                BroadcastTCP(clientInfo);

                if (!clientInfo.initialized)
                {
                    if (clientInfo.externalEndPoint != null & protocol == ProtocolType.Udp)
                        SendUdp(new Common.Message("Server", clientInfo.name, "UDP Communication Test"), IPEndPoint.Parse(clientInfo.externalEndPoint));
                    if (clientInfo.client != null & protocol == ProtocolType.Tcp)
                        SendTcp(new Common.Message("Server", clientInfo.name, "TCP Communication Test"), clientInfo.client);

                    if (clientInfo.client != null & clientInfo.externalEndPoint != null)
                    {
                        foreach (ClientInfo info in clients)
                            SendUdp(info, IPEndPoint.Parse(clientInfo.externalEndPoint));

                        clientInfo.initialized = true;
                    }
                }
            }
            else if (package.typename == "Message")
            {
                window.outToLog($"Message from {tcpEndpoint.Address}:{tcpEndpoint.Port}: {((Common.Message)package).content}");
            }
            else if (package.typename == "Req")
            {
                Req R = (Req)package;

                ClientInfo clientInfo = clients.FirstOrDefault(x => x.id == R.recipientId);

                if (clientInfo != null)
                    SendTcp(R, clientInfo.client);
            }
        }

        private void SendTcp(IPackaged package, TcpClient client)
        {
            if (client != null && client.Connected)
            {
                byte[] data = Serializer.serializePackage(package);

                NetworkStream NetStream = client.GetStream();
                NetStream.Write(data, 0, data.Length);
            }
        }

        static void SendUdp(IPackaged package, IPEndPoint EP)
        {
            byte[] data = Serializer.serializePackage(package);
            udp.Send(data, data.Length, udpEndpoint);
        }

        private void BroadcastTCP(IPackaged package)
        {
            foreach (ClientInfo clientInfo in clients.Where(x => x.client != null))
                SendTcp(package, clientInfo.client);
        }

        private void LogHostIp()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addr = ipEntry.AddressList;
            window.outToLog($"Server address: {addr[addr.Length - 1]}");
        }

    }
}
