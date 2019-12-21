using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kontra_Music_Player
{
    public partial class Form2 : Form
    {

        Point lastClick;
        public Form2()
        {
            InitializeComponent();
        }

        public String getIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label1.Text = "Server on " + getIpAddress() + ":8060";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Form2_MouseDown(sender, e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Form2_MouseMove(sender, e);
        }

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            lastClick = new Point(e.X, e.Y); //We'll need this for when the Form starts to move
        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //Only when mouse is clicked
            {
                //Move the Form the same difference the mouse cursor moved;
                this.Left += e.X - lastClick.X;
                this.Top += e.Y - lastClick.Y;
            }
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\exitbut.png");
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\minbut.png");
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\exithover.png");
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.BackgroundImage = Image.FromFile(Application.StartupPath + "\\images\\minhover.png");
        }
    }
}
