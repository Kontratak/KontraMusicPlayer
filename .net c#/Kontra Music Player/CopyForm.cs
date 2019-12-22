using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kontra_Music_Player
{
    public partial class CopyForm : Form
    {

        Point lastClick;
        int length;
        int i = 0;

        public CopyForm(int length)
        {
            InitializeComponent();
            this.length = length;
        }

        public void toProgress(String musicname,int copylength)
        {
            label3.Text = musicname;
            progressBar1.Value += 1;
            wait(100);
            label4.Text = "%" + ((progressBar1.Value * 100) / progressBar1.Maximum);
            i++;
            if(i == length)
            {
                progressBar1.Value = progressBar1.Maximum;
                label3.Text = "";
                label2.Text = "Kopyalama Tamamlandı";
                wait(2000);
                this.Close();
            }
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
            CopyForm_MouseDown(sender, e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
           CopyForm_MouseMove(sender, e);
        }

        private void CopyForm_MouseDown(object sender, MouseEventArgs e)
        {
            lastClick = new Point(e.X, e.Y); //We'll need this for when the Form starts to move
        }

        private void CopyForm_MouseMove(object sender, MouseEventArgs e)
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

        private void CopyForm_Load(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = length;
        }

        public static void wait(int milliseconds)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }

    }
}
