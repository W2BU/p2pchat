using p2pchat.ServerCore;


namespace p2pchat
{
    public partial class ServerWindow : Form
    {
        private Server server;
        public ServerWindow()
        {
            InitializeComponent();
            server = new Server(this);
        }

        private void serverStopButton_Click(object sender, EventArgs e)
        {
            server.Stop();
            Environment.Exit(0);
        }

        public void OutToLog(string output)
        {
            if (serverLogBox.InvokeRequired)
            {
                serverLogBox.Invoke(new MethodInvoker(() => OutToLog(output)));
                return;
            }
            serverLogBox.AppendText("\r\n" + output);
            serverLogBox.ScrollToCaret();
        }

        private void ServerWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Stop();
            Environment.Exit(0);
        }
    }
}
