using p2pchat.ClientCore;
using p2pchat.Common;
using p2pchat.Forms;

namespace p2pchat
{
    public partial class ClientWindow : Form
    {
        private Client client;

        List<ClientInfo> availableUsers = new List<ClientInfo>();
        List<ChatWindow> chatWindowList = new List<ChatWindow>();
        public ClientWindow()
        {
            InitializeComponent();
            client = new Client(this);
        }

        private void chatButton_Click(object sender, EventArgs e)
        {
            if (availableUsersList.SelectedItem != null)
            {
                ClientInfo CI = availableUsers.FirstOrDefault(x => x.name == availableUsersList.SelectedItem.ToString());
                client.ConnectToClient(CI);
            }
        }
        
        private void ClientWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            availableUsersList.Items.Clear();
            for (int c = 0; c < chatWindowList.Count - 1; c++)
                chatWindowList[c].Close();
            Environment.Exit(0);
        }

        public void addClient(ClientInfo clientInfo)
        {
            if (clientStatusBox.InvokeRequired)
            {
                clientStatusBox.Invoke(new MethodInvoker(() => addClient(clientInfo)));
                return;
            }
            availableUsersList.Items.Add(clientInfo.name);
            availableUsers.Add(clientInfo);
        }

        public void removeClient(ClientInfo clientInfo)
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
                clientStatusBox.Invoke(new MethodInvoker(() => removeClient(clientInfo)));
                return;
            }

            if (i != -1)
                availableUsersList.Items.RemoveAt(i);
                availableUsers.Remove(availableUsers.FirstOrDefault(x => x.name == availableUsersList.SelectedItem.ToString()));

            if (Chat != null)
                Chat.Close();

        }

        public void outToLog(string output)
        {
            if (clientStatusBox.InvokeRequired)
            {
                clientStatusBox.Invoke(new MethodInvoker(() => outToLog(output)));
                return;
            }
            clientStatusBox.AppendText("\r\n" + output);
            clientStatusBox.ScrollToCaret();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (enterUsernameBox.Text.Length != 0)
            {
                client.UpdateUserName(enterUsernameBox.Text);
            }
            else if (enterServerAddressBox.Text.Length != 0)
            {
                client.UpdateServerAddress(enterServerAddressBox.Text);
            }
            client.Connect();
        }
    }
}