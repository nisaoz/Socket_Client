using System;
using System.Threading;
using System.Windows.Forms;

namespace Socket_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.Visible = false;
            this.ShowInTaskbar = false;

            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            notifyIcon_client.Icon = Properties.Resources.Bomberman;
            notifyIcon_client.Visible = true;

            try
            {
                Thread th = new Thread(new ThreadStart(SocketClient_11500.Start));
                th.Start();
                //SocketClient_11500.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                Application.Exit();
            }
        }

        private void notifyIcon_client_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
                contextMenuStrip.Show(MousePosition);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}