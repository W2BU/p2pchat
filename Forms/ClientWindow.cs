using p2pchat.ClientCore;
using p2pchat.Common;
using p2pchat.Forms;
using System.Net;
using static p2pchat.ClientCore.Client;

namespace p2pchat
{
    public partial class ClientWindow : Form
    {
        private Client localClient;

        List<ClientInfo> availableUsers = new List<ClientInfo>();
        List<ChatWindow> chatWindowList = new List<ChatWindow>();
        public ClientWindow()
        {
            InitializeComponent();
            localClient = new Client(this);
        }

        private void chatButton_Click(object sender, EventArgs e)
        {
            if (availableUsersList.SelectedItem != null)
            {
                ClientInfo CI = availableUsers.FirstOrDefault(x => x.name == availableUsersList.SelectedItem.ToString());
                localClient.ConnectToClient(CI);
            }
        }
        
        private void ClientWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            availableUsersList.Items.Clear();
            for (int c = 0; c < chatWindowList.Count - 1; c++)
                chatWindowList[c].Close();
            Environment.Exit(0);
        }

        public void AddClient(ClientInfo clientInfo)
        {
            if (clientStatusBox.InvokeRequired)
            {
                clientStatusBox.Invoke(new MethodInvoker(() => AddClient(clientInfo)));
                return;
            }
            if (clientInfo.id != localClient.localClientInfo.id)
            {
                availableUsersList.Items.Add(clientInfo.name);
                availableUsers.Add(clientInfo);
            }

        }

        public void Disconnect(Client client)
        {
            connectButton.Text = "Connect";
            connectButton.Update();
            availableUsers.Clear();
            foreach (ChatWindow c in chatWindowList) c.Close();
        }

        public void RemoveClient(ClientInfo clientInfo)
        {
            int i = -1;
            ChatWindow Chat = null;

            foreach (ClientInfo CI in availableUsersList.Items)
                if (CI.id == clientInfo.id)
                    i = availableUsersList.Items.IndexOf(clientInfo.name);

            foreach (ChatWindow CW in chatWindowList)
                if (CW.id == clientInfo.id)
                    Chat = CW;

            if (clientStatusBox.InvokeRequired)
            {
                clientStatusBox.Invoke(new MethodInvoker(() => RemoveClient(clientInfo)));
                return;
            }

            if (i != -1)
                availableUsersList.Items.RemoveAt(i);
                availableUsers.Remove(availableUsers.FirstOrDefault(x => x.name == availableUsersList.SelectedItem.ToString()));

            if (Chat != null)
                Chat.Close();

        }

        public void InitiateDialogue(ClientInfo sender, IPEndPoint endPoint)
        {
            Invoke(new MethodInvoker(() =>
            {
                ChatWindow chat = chatWindowList.FirstOrDefault(C => C.endPoint.Equals(endPoint));

                if (chat == null)
                {
                    chat = new ChatWindow(localClient, sender.name, endPoint, sender.id);
                    chatWindowList.Add(chat);
                    chat.Closed += delegate { chatWindowList.Remove(chat); };
                    chat.Show();
                }
                else
                {
                    chat.Focus();
                    chat.BringToFront();
                }
            }));
        }

        public void ReceiveDialogue(IPEndPoint sender, MessageReceivedEventArgs messageArgs)
        {
            Invoke(new MethodInvoker(() =>
            {
                ChatWindow chat = chatWindowList.FirstOrDefault(C => C.endPoint.Equals(sender));

                if (chat == null)
                {
                    chat = new ChatWindow(localClient, messageArgs.clientInfo.name, messageArgs.endPoint, messageArgs.clientInfo.id);
                    chatWindowList.Add(chat);
                    chat.Closed += delegate { chatWindowList.Remove(chat); };
                    chat.Show();
                }
                else
                {
                    chat.Focus();
                    chat.BringToFront();
                }

                chat.ReceiveMessage(messageArgs.message);
            }));
            
        }

        public void OutToLog(string output)
        {
            if (clientStatusBox.InvokeRequired)
            {
                clientStatusBox.Invoke(new MethodInvoker(() => OutToLog(output)));
                return;
            }
            clientStatusBox.AppendText("\r\n" + output);
            clientStatusBox.ScrollToCaret();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (enterUsernameBox.Text.Length != 0)
            {
                localClient.UpdateUserName(enterUsernameBox.Text);
            }
            else if (enterServerAddressBox.Text.Length != 0)
            {
                localClient.UpdateServerAddress(enterServerAddressBox.Text);
            }
            localClient.ConnectDisconnect();
            _ = connectButton.Text == "Disconnect" ? connectButton.Text = "Connect" : connectButton.Text = "Disconnect";
            connectButton.Update();
        }
    }
}