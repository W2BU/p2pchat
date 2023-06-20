using p2pchat.ServerCore;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Schema;

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
        }

        public void outToLog(string output)
        {
            if (serverLogBox.InvokeRequired)
            {
                serverLogBox.Invoke(new MethodInvoker(() => outToLog(output)));
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
