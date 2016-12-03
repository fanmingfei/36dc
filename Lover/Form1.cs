using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace Lover
{
    public partial class Form1 : Form
    {
        private FolderBrowserDialog fromDirectory = new FolderBrowserDialog();
        private FolderBrowserDialog toDirectory = new FolderBrowserDialog();
        private String fromPath;
        private String toPath;
        private Int32 sleep;
        private Thread startSleepThread;
        public Form1()
        {
            InitializeComponent();
        }
        internal static int WM_NCHITTEST = 0x84;
        internal static IntPtr HTCLIENT = (IntPtr)0x1;
        internal static IntPtr HTCAPTION = (IntPtr)0x2;
        internal static int WM_NCLBUTTONDBLCLK = 0x00A3;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                return;
            }
            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if (m.Result == HTCLIENT)
                {
                    m.HWnd = this.Handle;
                    m.Result = HTCAPTION;
                }
                return;
            }
            base.WndProc(ref m);
        }
        private void CopyDirectory(string srcdir, string desdir)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);

            string desfolderdir = desdir + "\\" + folderName;

            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
            {
                desfolderdir = desdir + folderName;
            }
            string[] filenames = Directory.GetFileSystemEntries(srcdir);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {

                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }

                    CopyDirectory(file, desfolderdir);
                }

                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);

                    srcfileName = desfolderdir + "\\" + srcfileName;


                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }

                    File.Copy(file, srcfileName);
                }
            }//foreach 
        }//function end

        private void button1_Click(object sender, EventArgs e)
        {
            fromDirectory.ShowDialog();
            fromInput.Text = fromDirectory.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            toDirectory.ShowDialog();
            toInput.Text = toDirectory.SelectedPath;
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader("save.dc", Encoding.UTF8);
            String line;
            line = sr.ReadLine();
            sr.Close();
            if (line == null) return;
            char[] chr = new char[] { '|' };
            string[] Paths = line.Split(chr);
            if (Paths[0] != null)
            {
                fromInput.Text = Paths[0];
            }
            if (Paths[1] != null)
            {
                toInput.Text = Paths[1];
            }


        }


        private void startSleep()
        {
            while(true)
            {
                Thread thread = new Thread(doCopy);
                thread.Start();
                System.Console.WriteLine("复制开始！");
                Thread.Sleep(sleep * 1000 * 60);
            }
        }
        private void doCopy ()
        {

            CopyDirectory(fromPath, toPath);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sleep = Convert.ToInt32(sleepTime.Text);
            fromPath = fromInput.Text;
            toPath = toInput.Text + "\\" + DateTime.Now.ToString("yyyyMMddhhmmss");

            if (toInput.Text == fromInput.Text)
            {
                MessageBox.Show("两个文件夹不能选取同一个");
                return;
            }
            if (sleep == 0 || fromPath == "" || toInput.Text == "")
            {
                MessageBox.Show("请完善信息哦~");
                return;
            }

            FileStream fs = new FileStream("save.dc", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(fromInput.Text + "|" + toInput.Text);
            sw.Flush();
            sw.Close();
            fs.Close();

            startSleepThread = new Thread(startSleep);
            startSleepThread.Start();
            label9.Text = "36℃的我正在以每" + sleep + "分钟一次的频率为您备份！";
            customTabControl1.SelectedTab = tabPage2;
        }

        private void notifyIcon_MouseDoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                this.Activate();
                this.ShowInTaskbar = true;
                this.Show();
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }


        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            startSleepThread.Abort();
            customTabControl1.SelectedTab = tabPage1;
        }

    }
}
