using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Eneter.Messaging.EndPoints.TypedMessages;
using Eneter.Messaging.MessagingSystems.MessagingSystemBase;
using Eneter.Messaging.MessagingSystems.TcpMessagingSystem;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Shell;
using Cake.Core.IO;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace Kontra_Music_Player
{
    public partial class Form1 : Form
    {

        OpenFileDialog opf = new OpenFileDialog();
        int opfclick = 0;
        double timeleft = 0;
        int index = -2;
        int clicktimes = 0;
        int clicktimespl = 0;
        String muziktext = "";
        String path = "";
        List<String> paths = new List<string>();
        List<String> times = new List<string>();
        List<String> list = new List<String>();
        List<String> savepaths = new List<String>();
        List<String> commands = new List<String>();
        bool activated = false;
        Button btnsaveplaylist = new Button();
        Button btnclose = new Button();
        Label label = new Label();
        int width = 0;
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        bool showlist = false;
        private static IDuplexTypedMessageReceiver<MyResponse, MyRequest> myReceiver;
        private delegate void SafeCallDelegate(string text);
        private delegate void SafeCallDelegate2(int index);
        private delegate void SafeCallDelegate3();
        //SpeechSynthesizer ss = new SpeechSynthesizer();
        //SpeechRecognitionEngine sre;
        String receiverid = null;
        String playstate = "paused";
        public delegate void MyDelegate(string input);
        StreamWriter w = File.AppendText("log.txt");
        Form2 form2;
        bool canaddtoplaylist = false;
        int volumemutetimes = 0;
        int volume = 0;
        Point lastClick;
        PlaylistForm pf;
        bool playagain = false;
        public void readySpeechEngine()
        {
            //ss.SetOutputToDefaultAudioDevice();
            //CultureInfo ci = new CultureInfo("en-GB");
            //sre = new SpeechRecognitionEngine(ci);
            //sre.SetInputToDefaultAudioDevice();
            //sre.SpeechRecognized += sre_SpeechRecognized;
            //string yol = "komut.txt";
            //using (StreamReader sr = new StreamReader(yol))
            //{
            //    while (!sr.EndOfStream)
            //    {
            //        commands.Add(sr.ReadLine());
            //    }
            //}
            //foreach (String command in commands)
            //{
            //    Choices choices = new Choices(command);
            //    GrammarBuilder gbuilder = new GrammarBuilder(choices);
            //    Grammar grammar = new Grammar(gbuilder);
            //    sre.LoadGrammarAsync(grammar);
            //}
            //sre.RecognizeAsync(RecognizeMode.Multiple);

        }

        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

        //    if (activated == true)
        //    {
        //        if (e.Result.Confidence > 0.5)
        //        {
        //            if (e.Result.Text == "play")
        //                button1.PerformClick();
        //            else if (e.Result.Text == "stop")
        //                button2.PerformClick();
        //            else if (e.Result.Text == "pause" || e.Result.Text == "resume")
        //                button5.PerformClick();
        //            else if (e.Result.Text == "next")
        //                button7.PerformClick();
        //            else if (e.Result.Text == "back")
        //                button6.PerformClick();
        //            else if (e.Result.Text == "open")
        //                button4.PerformClick();
        //            else if (e.Result.Text == "up")
        //                VolumeUp();
        //            else if (e.Result.Text == "down")
        //                VolumeDown();
        //            else if (e.Result.Text == "max")
        //                VolumeMax();
        //            else if (e.Result.Text == "half")
        //                VolumeHalf();
        //            else if (e.Result.Text == "mute")
        //                VolumeMute();
        //            Console.WriteLine(e.Result.Text);
        //        }
        //}

        }

        public Form1()
        {
            InitializeComponent();
            axWindowsMediaPlayer2.settings.volume = 100;
        }

        public Form1(string path)
        {
            InitializeComponent();
            if (path.EndsWith(".kpl"))
            {
                addPltoList(path);
            }
            else
            {
                this.path = path;
                axWindowsMediaPlayer2.settings.volume = 100;
                String[] name = path.Split('\\');
                listView1.Items.Add(name[name.Length - 1]);
                paths.Add(path);
                list.Add(name[name.Length - 1]);
                listView1.Items[0].Selected = true;
                button1.Enabled = true;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            opf.Multiselect = true;
            opf.Title = "Browse Audio Files";
            opf.FileName = "";
            opf.Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav|All files (*.*)|*.*";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                if (opf.FileNames[0].EndsWith("kpl"))
                {
                    addPltoList(opf.FileNames[0]);
                    return;
                }
                foreach (String names in opf.FileNames)
                {
                    using (ShellObject shell = ShellObject.FromParsingName(names))
                    {
                        IShellProperty prop = shell.Properties.System.Media.Duration;
                        var t = (ulong)prop.ValueAsObject;
                        TimeSpan s = TimeSpan.FromTicks((long)t);
                        if (opfclick == 0)
                        {
                            String[] name = names.Split('\\');
                            paths.Add(names);
                            String name2 = name[name.Length - 1].Substring(0, name[name.Length - 1].Length - 4);
                            list.Add(name2);
                            String time = s.ToString().Substring(3, 5);
                            times.Add(time);
                            String[] values = { name2, time };
                            ListViewItem add = new ListViewItem(values);
                            listView1.Items.Add(add);
                        }
                        else
                        {
                            String[] name = names.Split('\\');
                            if (!(list.Contains(name[name.Length - 1])))
                            {
                                paths.Add(names);
                                String name2 = name[name.Length - 1].Substring(0, name[name.Length - 1].Length - 4);
                                list.Add(name2);
                                String time = s.ToString().Substring(3, 5);
                                times.Add(time);
                                String[] values = { name2, time };
                                ListViewItem add = new ListViewItem(values);
                                listView1.Items.Add(add);
                            }
                        }
                    }
                }
            }
            opfclick = 1;
            button1.Enabled = true;
            if (receiverid != null)
                sendPlaylist();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Enabled)
            {
                play();
            }

        }

        public void play()
        {
            if (index == -1)
            {
                label1.Text = "Choose a Song to Play";
                if (axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    wait(3000);
                    label1.Text = muziktext;
                }
            }
            else
            {
                if (index == -2) index = 0;
                axWindowsMediaPlayer2.URL = paths[index];
                axWindowsMediaPlayer2.Ctlcontrols.play();
                label1.Text = list[index];
                button5.Enabled = true;
                trackBar1.Value = 0;
                timer1.Interval = 1000;
                timer1.Enabled = true;
                clicktimes = 0;
                button5.Image = Image.FromFile(Application.StartupPath + "\\images\\pause.png");
                timer1.Start();
                timer2.Start();
                playstate = "playing";
                if (receiverid != null)
                {
                    sendMusic();
                    sendPlayState();
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer2.Ctlcontrols.stop();
            timer2.Stop();
            playstate = "paused";
            if (receiverid != null)
                sendPlayState();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pauseresume();
        }

        protected void pauseresume()
        {
            if (index != -1)
            {
                if (clicktimes % 2 == 1)
                {
                    axWindowsMediaPlayer2.Ctlcontrols.play();
                    button5.Image = Image.FromFile(Application.StartupPath + "\\images\\pause.png");
                    timer2.Start();
                    playstate = "playing";
                    if (receiverid != null)
                        sendPlayState();
                }
                else
                {
                    axWindowsMediaPlayer2.Ctlcontrols.pause();
                    button5.Image = Image.FromFile(Application.StartupPath + "\\images\\resume.png");
                    timer2.Stop();
                    playstate = "paused";
                    if (receiverid != null)
                        sendPlayState();
                }
                if (receiverid != null)
                    sendMusic();
                clicktimes++;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = axWindowsMediaPlayer2.Ctlcontrols.currentPositionString;
            trackBar1.Value = (int)axWindowsMediaPlayer2.Ctlcontrols.currentPosition;
            if (string.IsNullOrEmpty(axWindowsMediaPlayer2.Ctlcontrols.currentPositionString))
            {
                if (axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsStopped && timeleft == axWindowsMediaPlayer2.currentMedia.duration)
                {
                    if (playagain == false)
                    {
                        index = listView1.SelectedIndices[0] + 1;
                        if (!(index == listView1.Items.Count))
                        {
                            axWindowsMediaPlayer2.URL = paths[index];
                            axWindowsMediaPlayer2.Ctlcontrols.play();
                            label1.Text = list[index];
                            listView1.Items[index].Selected = true;
                        }
                        else
                        {
                            button7.PerformClick();
                        }
                    }
                    else
                    {
                        index = listView1.SelectedIndices[0];
                        axWindowsMediaPlayer2.URL = paths[index];
                        axWindowsMediaPlayer2.Ctlcontrols.play();
                        label1.Text = list[index];
                        listView1.Items[index].Selected = true;
                    }
                }
            }

        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            button1.Enabled = true;
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (s[0].EndsWith(".kpl"))
            {
                addPltoList(s[0]);
                return;
            }
            foreach (String names in s)
            {
                String[] name = names.Split('\\');
                    String name2 = name[name.Length - 1].Substring(0, name[name.Length - 1].Length - 4);
                if (!list.Contains(name2))
                {
                    paths.Add(names);
                    list.Add(name2);
                    listView1.Items.Add(name2);
                    listView1.Items[index].Selected = true;
                }
                    
            }
            if (receiverid != null)
                sendPlaylist();

        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            width = this.Width;
            if (!(string.IsNullOrEmpty(path)))
                button1_Click(null, null);
            addAllToPlaylistToolStripMenuItem1.Enabled = false;
            addToPlaylistToolStripMenuItem1.Enabled = false;
            

        }

        private void Pf_FormClosed(object sender, FormClosedEventArgs e)
        {
            canaddtoplaylist = false;
            createANewPlaylistToolStripMenuItem1.Enabled = true;
            addAllToPlaylistToolStripMenuItem1.Enabled = false;
            addToPlaylistToolStripMenuItem1.Enabled = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button4.PerformClick();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button3.PerformClick();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            safeFormClose();
            Application.Exit();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (button1.Enabled)
                button1.PerformClick();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button5.PerformClick();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2.PerformClick();
            ;
            if (receiverid != null)
                sendMusic();
        }

        private void muteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            trackBar2.Value = 0;
            axWindowsMediaPlayer2.settings.volume = 0;
            Image m = Image.FromFile(Application.StartupPath + "\\images\\sound0.png");
            pictureBox2.Image = m;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (index > 0)
                index--;

            else
                index = listView1.Items.Count - 1;
            if (listView1.Items.Count > 0)
            {
                listView1.Items[index].Selected = true;
                button1.PerformClick();
                muziktext = list[index];
            }
            if (receiverid!=null)
            {
                
                sendMusic();
            }
            playstate = "playing";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (index < listView1.Items.Count - 1)
                index++;
            else
                index = 0;
            if (listView1.Items.Count > 0)
            {
                listView1.Items[index].Selected = true;
                button1.PerformClick();
                muziktext = list[index];
            }
            if (receiverid!=null)
            {
                
                sendMusic();
            }
            playstate = "playing";
        }


        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listView1.SelectedItems == null)
                e.Cancel = true;
        }


        protected void savePlaylist(List<String> paths)
        {
            if (index >= 0)
            {
                saveFileDialog1.Filter = "Kontra Playlist file (*.kpl)|*.kpl";
                saveFileDialog1.FileName = "Playlist1.kpl";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                    {
                        foreach (String path in paths)
                            sw.WriteLine(path);
                    }

                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Image flipImage = pictureBox1.Image;
            flipImage.RotateFlip(RotateFlipType.Rotate270FlipXY);
            pictureBox1.Image = flipImage;
        }

        private void addPltoList(String path)//Add items to playlist in dragdrop event
        {
            path.Replace("/", "\\\\");
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    String names = sr.ReadLine();
                    using (ShellObject shell = ShellObject.FromParsingName(names))
                    {
                        IShellProperty prop = shell.Properties.System.Media.Duration;
                        var t = (ulong)prop.ValueAsObject;
                        TimeSpan s = TimeSpan.FromTicks((long)t);
                        paths.Add(names);
                        String[] name = names.Split('\\');
                        String name2 = name[name.Length - 1].Substring(0, name[name.Length - 1].Length - 4);
                        if (!list.Contains(name2))
                        {
                            list.Add(name2);
                            listView1.Items.Add(name2);
                            listView1.Items[0].Selected = true;
                            button1.Enabled = true;
                        }
                    }
                }
            }
            button1_Click(null, null);
            if(receiverid!=null)
                sendPlaylist();
        }

        private void VolumeUp()
        {
            axWindowsMediaPlayer2.settings.volume += 10;
            if (trackBar2.Value <= 90)
                trackBar2.Value += 10;
        }
        private void VolumeDown()
        {
            axWindowsMediaPlayer2.settings.volume -= 10;
            if (trackBar2.Value >= 10)
                trackBar2.Value -= 10;
        }
        private void VolumeHalf()
        {
            axWindowsMediaPlayer2.settings.volume = 50;
            trackBar2.Value = 50;
        }
        private void VolumeMax()
        {
            axWindowsMediaPlayer2.settings.volume = 100;
            trackBar2.Value = 100;
        }
        private void VolumeMute()
        {
            axWindowsMediaPlayer2.settings.volume = 0;
            trackBar2.Value = 0;
        }

        private void activatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readySpeechEngine();
            activated = true;
            activatedToolStripMenuItem.Enabled = false;
            deactivateToolStripMenuItem.Enabled = true;
        }

        private void deactivateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activated = false;
            activatedToolStripMenuItem.Enabled = true;
            deactivateToolStripMenuItem.Enabled = false;
        }

        private void getConnection()
        {

            // Create message receiver receiving 'MyRequest' and receiving 'MyResponse'.
            IDuplexTypedMessagesFactory aReceiverFactory = new DuplexTypedMessagesFactory();
            myReceiver = aReceiverFactory.CreateDuplexTypedMessageReceiver<MyResponse, MyRequest>();

            // Subscribe to handle messages.
            myReceiver.MessageReceived += OnMessageReceived;
            form2 = new Form2();
            // Create TCP messaging.
            IMessagingSystemFactory aMessaging = new TcpMessagingSystemFactory();
            IDuplexInputChannel anInputChannel
                = aMessaging.CreateDuplexInputChannel("tcp://"+form2.getIpAddress()+":8060/");
            //= aMessaging.CreateDuplexInputChannel("tcp://192.168.173.1:8060/");
            // Attach the input channel and start to listen to messages.
            myReceiver.AttachDuplexInputChannel(anInputChannel);
            form2.ShowDialog();
            Console.WriteLine("The service is running. To stop press enter.");
            Console.ReadLine();
            // Detach the input channel and stop listening.
            // It releases the thread listening to messages.
            myReceiver.DetachDuplexInputChannel();
        }

        private void sendMusic()
        {
            MyResponse aResponse = new MyResponse();
            aResponse.Text = label1.Text;
            aResponse.Length = 2;
            if (myReceiver.IsDuplexInputChannelAttached && this.receiverid != null)
                try
                {
                    myReceiver.SendResponseMessage(receiverid, aResponse);
                }
                catch (InvalidOperationException ex) { }
            Console.WriteLine("Text send : " + aResponse.Text);
            Log(aResponse.Text, "Send");
        }
        private void sendPlayState()
        {
            MyResponse response = new MyResponse();
            if (myReceiver.IsDuplexInputChannelAttached && this.receiverid != null)
            {
                if (playstate == "playing")
                {
                    response.Length = 7;
                    response.Text = "playing";
                    
                }
                else
                {
                    response.Length = 8;
                    response.Text = "paused";
                }
                try
                {
                    myReceiver.SendResponseMessage(receiverid, response);
                }
                catch (InvalidOperationException ex) { }
                Console.WriteLine("Text send : " + response.Text);
                Log(response.Text, "Send");
            }
        }
        private void sendPlaylist()
        {
            MyResponse response = new MyResponse();
            response.Length = 9;
            foreach(String l in list)
            {
                response.Text = l;
                myReceiver.SendResponseMessage(receiverid, response);
            }
            //TODO
        }

        private void closeConnection()
        {
            receiverid = null;
        }

        private void OnMessageReceived(object sender, TypedRequestReceivedEventArgs<MyRequest> e)
        {

            MyResponse aResponse = new MyResponse();
            aResponse.Text = label1.Text;
            aResponse.Length = 2;
            Console.WriteLine("Text Received : "+e.RequestMessage.Text);
            Log(aResponse.Text, "Received");
            try
            {
                myReceiver.SendResponseMessage(e.ResponseReceiverId, aResponse);
            }
            catch (InvalidOperationException ex) { }
                
            if (e.RequestMessage.Text == "pause")
            {
                safepauseresume();
            }
            else if (e.RequestMessage.Text == "next")
            {
                if (index < listView1.Items.Count - 1)
                    index++;
                else
                    index = 0;
                safenext("0");
            }
            else if (e.RequestMessage.Text == "prev")
            {
                if (index > 0)
                    index--;
                else
                    index = listView1.Items.Count - 1;
                safeprev("0");
            }
            //TODO
            else if (e.RequestMessage.Text == "connected")
            {
                isPlaying(e.ResponseReceiverId);
                sendMusic();
            }
            else if (e.RequestMessage.Text == "volumeup")
            {
                if (axWindowsMediaPlayer2.settings.volume <= 90)
                    axWindowsMediaPlayer2.settings.volume += 10;
                safevolumeup("0");
            }
            else if (e.RequestMessage.Text == "volumedown")
            {
                if (axWindowsMediaPlayer2.settings.volume >= 10)
                    axWindowsMediaPlayer2.settings.volume -= 10;
                safevolumedown("0");
            }
            else if (e.RequestMessage.Text == "disconnect")
                closeConnection();
            else if (e.RequestMessage.Text == "addplaylist")
            {
                MyResponse responsed = new MyResponse();
                if (canaddtoplaylist == true)
                {
                    addToPlaylistToolStripMenuItem1.PerformClick();
                    responsed.Length = 1;
                    myReceiver.SendResponseMessage(e.ResponseReceiverId, responsed);
                    Console.WriteLine("Num send : " + responsed);
                    Log(responsed.Length.ToString(), "Send");
                }
                else
                {
                    responsed.Length = 0;
                    myReceiver.SendResponseMessage(e.ResponseReceiverId, responsed);
                    Console.WriteLine("Num send : " + responsed);
                    Log(responsed.Length.ToString(), "Send");
                }
            }
            else if (e.RequestMessage.Text == "mute")
                safevolumemute("0");
            else if (e.RequestMessage.Text == "tenfor")
            {
                if(axWindowsMediaPlayer2.Ctlcontrols.currentPosition + 10 < axWindowsMediaPlayer2.currentMedia.duration)
                axWindowsMediaPlayer2.Ctlcontrols.currentPosition += 10;
            }
            else if (e.RequestMessage.Text == "tenback")
            {
                if (axWindowsMediaPlayer2.Ctlcontrols.currentPosition - 10 > 0)
                    axWindowsMediaPlayer2.Ctlcontrols.currentPosition -= 10;
            }
            else if (e.RequestMessage.Text == "playagain")
            {
                playagain = true;
            }
        }

        private void isPlaying(String receiverid)
        {
            this.receiverid = receiverid;
            MyResponse aResponse = new MyResponse();
            if(playstate == "playing")
            {
                aResponse.Text = "playing";
                aResponse.Length = 4;
            }
            else if(playstate == "paused")
            {
                aResponse.Text = "paused";
                aResponse.Length = 3;
            }
            myReceiver.SendResponseMessage(receiverid, aResponse);
            Console.WriteLine("Text send : " + aResponse.Text);
            Log(aResponse.Text,"Send");
        }
        public void Log(string logMessage,String status)
        {
            w.WriteLine(DateTime.Now.ToLongTimeString()+" : " + DateTime.Now.ToLongDateString() + " : " + "Text "+status+" : " + logMessage);
        }

        private void safeplay(string text)
        {
            if (index == -1)
            {
                label1.Text = "Choose a Song to Play";
                if (axWindowsMediaPlayer2.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    wait(3000);
                    label1.Text = muziktext;
                }
            }
            else
            {
                axWindowsMediaPlayer2.URL = paths[index];
                axWindowsMediaPlayer2.Ctlcontrols.play();       
                if (label1.InvokeRequired)
                {
                    var d = new SafeCallDelegate(safeplay);
                    label1.Invoke(d, new object[] { text });
                }
                else
                {
                    label1.Text = list[index];
                }
                if (trackBar1.InvokeRequired)
                {
                    var d = new SafeCallDelegate(safeplay);
                    trackBar1.Invoke(d, new object[] { text });
                }
                else
                {
                    trackBar1.Value = 0;
                }
                clicktimes = 0;
                button5.Image = Image.FromFile(Application.StartupPath + "\\images\\pause.png");
                timer1.Start();
                timer2.Start();
                playstate = "playing";
                if (receiverid != null)
                    sendMusic();
            }         
        }
        private void safenext(String text)
        {
            
            if (listView1.InvokeRequired)
            {
                var d = new SafeCallDelegate(safenext);
                listView1.Invoke(d, new object[] { text });
            }
            else
            {
                listView1.Items[index].Selected = true;
            }
            muziktext = list[index];
            safeplay("0");
            playstate = "playing";
            sendMusic();
        }
        private void safeprev(String text)
        {
            
            if (listView1.InvokeRequired)
            {
                var d = new SafeCallDelegate(safeprev);
                listView1.Invoke(d, new object[] { text });
            }
            else
            {
                listView1.Items[index].Selected = true;
            }
            muziktext = list[index];
            safeplay("0");
            playstate = "playing";
            sendMusic();
        }
        private void safevolumeup(String text)
        {
            if (trackBar2.InvokeRequired)
            {
                var d = new SafeCallDelegate(safevolumeup);
                trackBar2.Invoke(d, new object[] { text });
            }
            else
            {
                if (trackBar2.Value <= 90)
                    trackBar2.Value += 10;
            }
        }
        private void safevolumedown(String text)
        {
            if (trackBar2.InvokeRequired)
            {
                var d = new SafeCallDelegate(safevolumedown);
                trackBar2.Invoke(d, new object[] { text });
            }
            else
            {
                if (trackBar2.Value >= 10)
                    trackBar2.Value -= 10;
            }

        }
        private void safevolumemute(String text)
        {
            if(volumemutetimes %2 == 0)
            {
                if (trackBar2.InvokeRequired)
                {
                    var d = new SafeCallDelegate(safevolumedown);
                    trackBar2.Invoke(d, new object[] { text });
                }
                else
                {
                    volume = trackBar2.Value;
                    trackBar2.Value = 0;
                    axWindowsMediaPlayer2.settings.volume = 0;
                }
            }
            else
            {
                if (trackBar2.InvokeRequired)
                {
                    var d = new SafeCallDelegate(safevolumedown);
                    trackBar2.Invoke(d, new object[] { text });
                }
                else
                {
                    trackBar2.Value = volume;
                    axWindowsMediaPlayer2.settings.volume = volume;
                }
            }
            volumemutetimes++;

        }
        private void safepauseresume()
        {
            if (index != -1)
            {
                if (clicktimes % 2 == 1)
                {
                    axWindowsMediaPlayer2.Ctlcontrols.play();
                    button5.Image = Image.FromFile(Application.StartupPath + "\\images\\pause.png");
                    timer2.Start();
                    playstate = "playing";
                }
                else
                {
                    axWindowsMediaPlayer2.Ctlcontrols.pause();
                    button5.Image = Image.FromFile(Application.StartupPath + "\\images\\resume.png");
                    timer2.Stop();
                    playstate = "paused";
                }
                if (receiverid != null)
                    sendMusic();
                clicktimes++;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            safeFormClose(); 
        }

        private void safeFormClose()
        {
            if (form2 != null)
                if (form2.InvokeRequired)
                {
                    var d = new SafeCallDelegate3(safeFormClose);
                    form2.Invoke(d, new object[] { });
                }
                else
                    form2.Close();
            if(receiverid != null)
            {
                MyResponse aResponse = new MyResponse();
                aResponse.Text = "closed";
                aResponse.Length = 5;
                Console.WriteLine("Text Send : " + aResponse.Text);
                try
                {
                    myReceiver.SendResponseMessage(receiverid, aResponse);
                }
                catch (InvalidOperationException ex)
                {
                    int a = ex.Data.Count;
                }
                myReceiver.DetachDuplexInputChannel();
            }
            w.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(clicktimespl % 2 == 0)
            {
                button8.Image = Image.FromFile(Application.StartupPath + "\\images\\polygon5.png");
                panel3.SendToBack();
                panel4.BringToFront();
            }
            else
            {
                button8.Image = Image.FromFile(Application.StartupPath + "\\images\\polygon4.png");
                panel4.SendToBack();
                panel3.BringToFront();
                
            }
            clicktimespl++;
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem selection = listView1.GetItemAt(e.X, e.Y);
                if (selection != null)
                {
                    index = this.listView1.GetItemAt(e.X, e.Y).Index;
                    listView1.Items[index].Selected = true;
                }
                
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            index = this.listView1.GetItemAt(e.X, e.Y).Index;
            button1.PerformClick();
            if (index != -1)
            {
                muziktext = list[index];
            }
        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            axWindowsMediaPlayer2.Ctlcontrols.currentPosition = trackBar1.Value;
            int min = trackBar1.Value / 60;
            int sec = trackBar1.Value % 60;
            if (sec < 10)
            {
                label2.Text = "0" + min + ":" + "0" + sec;
            }
            else
            {
                label2.Text = "0" + min + ":" + sec;
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer2.settings.volume = trackBar2.Value;
            if (trackBar2.Value == 0)
            {
                Image m = Image.FromFile(Application.StartupPath + "\\images\\sound0.png");
                pictureBox2.Image = m;
            }
            else if (trackBar2.Value > 0 && trackBar2.Value <= 20)
            {
                Image m = Image.FromFile(Application.StartupPath + "\\images\\s20.png");
                pictureBox2.Image = m;
            }
            else if(trackBar2.Value > 20 && trackBar2.Value <= 40)
            {
                Image m = Image.FromFile(Application.StartupPath + "\\images\\s40.png");
                pictureBox2.Image = m;
            }
            else if (trackBar2.Value > 40 && trackBar2.Value <= 60)
            {
                Image m = Image.FromFile(Application.StartupPath + "\\images\\s60.png");
                pictureBox2.Image = m;
            }
            else if (trackBar2.Value > 60 && trackBar2.Value <= 80)
            {
                Image m = Image.FromFile(Application.StartupPath + "\\images\\s80.png");
                pictureBox2.Image = m;
            }
            else if (trackBar2.Value > 80 && trackBar2.Value <= 100)
            {
                Image m = Image.FromFile(Application.StartupPath + "\\images\\sfull.png");
                pictureBox2.Image = m;
            }
        }

        private void axWindowsMediaPlayer2_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            trackBar2.Value = axWindowsMediaPlayer2.settings.volume;
            if (e.newState == 3)
            {
                trackBar1.Maximum = (int)axWindowsMediaPlayer2.currentMedia.duration;
                label3.Text = axWindowsMediaPlayer2.currentMedia.durationString;
                playstate = "playing";
                if (receiverid != null)
                    sendMusic();
            }
            if (e.newState == 8)
            {
                timeleft = axWindowsMediaPlayer2.currentMedia.duration;
            }
        }

        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = listView1.Columns[e.ColumnIndex].Width;
        }

        private void openConnectionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(getConnection));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Form1_MouseDown(sender, e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Form1_MouseMove(sender, e);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            lastClick = new Point(e.X, e.Y); //We'll need this for when the Form starts to move
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) //Only when mouse is clicked
            {
                //Move the Form the same difference the mouse cursor moved;
                this.Left += e.X - lastClick.X;
                this.Top += e.Y - lastClick.Y;
            }
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.Image = Image.FromFile(Application.StartupPath + "\\images\\exitbut.png");
        }

        private void button9_MouseLeave(object sender, EventArgs e)
        {
            button9.Image = Image.FromFile(Application.StartupPath + "\\images\\minbut.png");
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (index != 0 && index != -1 && index != -2)
            {
                list.RemoveAt(index);
                paths.RemoveAt(index);
                listView1.Refresh();
                axWindowsMediaPlayer2.Ctlcontrols.stop();
                label1.Text = "Choose a Song to Play";
                listView1.Items.RemoveAt(index);
                timer2.Stop();
            }
            else if (index == 0)
            {
                list.RemoveAt(index);
                paths.RemoveAt(index);
                listView1.Refresh();
                axWindowsMediaPlayer2.Ctlcontrols.stop();
                label1.Text = "Choose a Song to Play";
                listView1.Items.RemoveAt(index);
                timer2.Stop();
                index = -1;
                if (receiverid != null)
                {
                    MyResponse r = new MyResponse();
                    r.Length = 6;
                    r.Text = "There is no music in playlist";
                    try
                    {
                        myReceiver.SendResponseMessage(receiverid, r);
                    }
                    catch (InvalidOperationException ex) { }
                }
            }
        }

        private void removeAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            paths.Clear();
            list.Clear();
            trackBar1.Value = 0;
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
            axWindowsMediaPlayer2.Ctlcontrols.stop();
            timer2.Stop();
            axWindowsMediaPlayer2.URL = "";
            index = -1;
            if (receiverid != null)
            {
                MyResponse r = new MyResponse();
                r.Length = 2;
                r.Text = "There is no music in playlist";
                try
                {
                    myReceiver.SendResponseMessage(receiverid, r);
                }
                catch (InvalidOperationException ex) { }
            }
        }

        private void addToPlaylistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pf.sendItem(list[index], paths[index], times[index]);
        }

        private void addAllToPlaylistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int i = 0;
            foreach (String listitem in list)
            {
                pf.sendItem(listitem, paths[i], times[i]);
                i++;
            }
        }

        private void createANewPlaylistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            canaddtoplaylist = true;
            pf = new PlaylistForm();
            pf.FormClosed += Pf_FormClosed;
            pf.Visible = true;
            createANewPlaylistToolStripMenuItem1.Enabled = false;
            addToPlaylistToolStripMenuItem1.Enabled = true;
            addAllToPlaylistToolStripMenuItem1.Enabled = true;
        }

        private void savePlaylstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savePlaylist(paths);
        }

        private void openPlaylistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            opf.Multiselect = false;
            opf.Title = "Browse Playlist File";
            opf.Filter = "Kontra Playlist files (*.kpl)|*.kpl";
            opf.FileName = "";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                listView1.Items.Clear();
                paths.Clear();
                list.Clear();
                label1.Text = "Choose a Song to Play";
                axWindowsMediaPlayer2.Ctlcontrols.stop();
                using (StreamReader sr = new StreamReader(opf.FileName))
                {
                    while (!sr.EndOfStream)
                    {
                        String names = sr.ReadLine();
                        try
                        {
                            using (ShellObject shell = ShellObject.FromParsingName(names))
                            {
                                IShellProperty prop = shell.Properties.System.Media.Duration;
                                var t = (ulong)prop.ValueAsObject;
                                TimeSpan s = TimeSpan.FromTicks((long)t);
                                paths.Add(names);
                                String[] name = names.Split('\\');
                                String name2 = name[name.Length - 1].Substring(0, name[name.Length - 1].Length - 4);
                                list.Add(name2);
                                String time = s.ToString().Substring(3, 5);
                                times.Add(time);
                                String[] values = { name2, time };
                                ListViewItem add = new ListViewItem(values);
                                listView1.Items.Add(add);
                                listView1.Items[0].Selected = true;
                                button1.Enabled = true;
                            }
                        }catch(Exception xe)
                        {
                            MessageBox.Show("Files Has Been Removed, Files Not Found!", "Warning");
                            break;
                        }
                    }
                    if (receiverid != null)
                        sendPlaylist();
                }
            }
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.Image = Image.FromFile(Application.StartupPath + "\\images\\exithover.png");
        }

        private void button9_MouseEnter(object sender, EventArgs e)
        {
            button9.Image = Image.FromFile(Application.StartupPath + "\\images\\minhover.png");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if(playagain == false)
            {
                button10.Image = Image.FromFile(Application.StartupPath + "\\images\\replay.png");
                playagain = true;
            }
            else
            {
                button10.Image = Image.FromFile(Application.StartupPath + "\\images\\dontreplay.png");
                playagain = false;
            }
            
        }
    }
    //Request Message Type
    public class MyRequest
    {
        public string Text { get; set; }
    }

    // Response message type
    public class MyResponse
    {
        public String Text { get; set; }
        public int Length { get; set; }
    }

}
