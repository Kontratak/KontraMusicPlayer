using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kontra_Music_Player
{
    public partial class PlaylistForm : Form
    {
        Point lastClick;
        List<String> listplaylist = new List<String>();
        List<String> savepaths = new List<String>();
        List<int> listmusiclength = new List<int>();
        Thread t;
        String sourcepath;
        CopyForm cp;
        public PlaylistForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void sendItem(String list, String path, String Length,int length)
        {
            if (!listplaylist.Contains(list))
            {
                listplaylist.Add(list);
                savepaths.Add(path);
                listmusiclength.Add(length);
                string[] values = { list, Length };
                ListViewItem add = new ListViewItem(values);
                listView1.Items.Add(add);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Kontra Playlist file (*.kpl)|*.kpl";
            saveFileDialog1.FileName = "Playlist1.kpl";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    foreach (String path in savepaths)
                        sw.WriteLine(path);
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.Image = Image.FromFile(Application.StartupPath + "\\images\\exithover.png");
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.Image = Image.FromFile(Application.StartupPath + "\\images\\exitbut.png");
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.Image = Image.FromFile(Application.StartupPath + "\\images\\minhover.png");
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.Image = Image.FromFile(Application.StartupPath + "\\images\\minbut.png");
        }

        private void PlaylistForm_MouseDown(object sender, MouseEventArgs e)
        {
            lastClick = new Point(e.X, e.Y); //We'll need this for when the Form starts to move
        }

        private void PlaylistForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //Only when mouse is clicked
            {
                //Move the Form the same difference the mouse cursor moved;
                this.Left += e.X - lastClick.X;
                this.Top += e.Y - lastClick.Y;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            PlaylistForm_MouseDown(sender, e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            PlaylistForm_MouseMove(sender, e);
        }

        private void savemusic_Click(object sender, EventArgs e)
        {
            t = new Thread(SaveMusic);
            folderBrowserDialog1.ShowDialog();
            sourcepath = folderBrowserDialog1.SelectedPath;
            t.Start();
            while (t.IsAlive) Application.DoEvents();

        }

        private void SaveMusic()
        {
            int i = 0;
            cp = new CopyForm(listplaylist.Count);
            cp.Visible = true;
            wait(2000);
            foreach (String filename in savepaths)
            {
                if(i == 0)
                {
                    cp.toProgress(listplaylist[i], 0);
                }
                else
                {
                    cp.toProgress(listplaylist[i], listmusiclength[i - 1]);
                }
                if (sourcepath != "" && filename != "")
                {
                    if (File.Exists(sourcepath + "\\" + listplaylist[i] + ".mp3")) ;
                    else
                    {
                        File.Copy(filename, sourcepath + "\\" + listplaylist[i] + ".mp3");
                    }

                }
                i++;
            }
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
