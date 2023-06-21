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

        public ChatWindow(Client _client, string _name, IPEndPoint _endPoint, long _id)
        {
            InitializeComponent();
            this.client = _client;
            this.name = _name;
            this.endPoint = _endPoint;
            this.id = _id;
            chatWithLabel.Text = $"Chat With {name}";
            chatWithLabel.Update();
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
