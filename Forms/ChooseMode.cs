using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace p2pchat
{
    public partial class ChooseMode : Form

    {
        public ChooseMode()
        {
            InitializeComponent();
        }

        private void serverBtn_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            ServerWindow serverWindow = new ServerWindow();
            serverWindow.Show();
        }

        private void clientBtn_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            ClientWindow clientWindow = new ClientWindow();
            clientWindow.Show();
        }

    }
}
