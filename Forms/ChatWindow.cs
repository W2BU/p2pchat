using p2pchat.ClientCore;
using System.Net;

namespace p2pchat.Forms
{
    public partial class ChatWindow : Form
    {
        public Client client;
        public string name;
        public IPEndPoint endPoint;
        public long id;

        public ChatWindow(Client client, string name, IPEndPoint endPoint, long id)
        {
            InitializeComponent();
            this.client = client;
            this.name = name;
            this.endPoint = endPoint;
            this.id = id;

            chatWithLabel.Text = $"Chat with {name}";
        }

        public void ReceiveMessage(Common.Message M)
        {
            dialogueBox.AppendText($"\r\n {M.from}: {M.content}");
            dialogueBox.ScrollToCaret();
            messageBox.Focus();
        }

        private void SendMessage()
        {
            Common.Message M = new Common.Message(client.localClientInfo.name, Name, messageBox.Text);
            client.SendMessageUdp(M, endPoint);
            dialogueBox.AppendText($"\r\n {client.localClientInfo.name}: {messageBox.Text}");
            dialogueBox.ScrollToCaret();
            messageBox.Text = string.Empty;
            messageBox.Focus();
        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }
    }
}
