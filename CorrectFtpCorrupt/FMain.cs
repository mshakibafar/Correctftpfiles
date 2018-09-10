using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorrectFtpCorrupt
{
    public partial class FMain : Form
    {
        List<string> filelist = new List<string>();
        public FMain()
        {
            InitializeComponent();
        }

        private void StartClick(object sender, EventArgs e)
        {
            filelist = Directory.GetFiles(txInputPath.Text, "*.*", SearchOption.AllDirectories).ToList<string>();

            if(filelist.Count==0)
            {
                MessageBox.Show("No file were found!");
                return;
            }

            DialogResult dr = MessageBox.Show(filelist.Count.ToString()+" files were found. Do you want to continue?",
                "Question" , MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                progressBar1.Maximum = filelist.Count;
                progressBar1.Value = 0;
                ThreadPool.QueueUserWorkItem(RunRepaid);
            }
        }

        private void RunRepaid(Object obj)
        {
            int c = 0;

            foreach (var item in filelist)
            {

                try
                {
                    CorrectFile(item);
                    AddText(item + " ---> is done.",c);
                    c++;
                }
                catch(Exception ex)
                {
;                    AddText(ex.ToString(), c);
                }
                

            }

            AddText(filelist.Count.ToString() + " file have repaired",progressBar1.Maximum);
        }

        delegate void AddTextDel(string v,int c);

        private void AddText(string item,int x)
        {
            if (richTextBox1.InvokeRequired)
                richTextBox1.Invoke(new AddTextDel(AddText), item,x);
            else
            {
                richTextBox1.AppendText( Environment.NewLine + item);
                richTextBox1.ScrollToCaret();
                progressBar1.Value = x;

            }
        }

        private void CorrectFile(string fname)
        {
            FileInfo fi = new FileInfo(fname);
            string OutFile = txOutpuPath.Text + "\\"+
                fi.DirectoryName.Replace("\\","_").Replace(":","_")+fi.Name;
            byte[] fileBYtes = File.ReadAllBytes(fname);

           // System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            string fileAsString = Encoding.Default.GetString(fileBYtes);

            // textBox2.Text = fileAsString;
            fileAsString = fileAsString.Replace("\r\n", "\n");
           // MessageBox.Show(CountSubstring(fileAsString, "\r\n").ToString());

            byte[] toBytes = Encoding.Default.GetBytes(fileAsString);

            File.WriteAllBytes(OutFile, toBytes);
          //  textBox2.Text += fname;
           // MessageBox.Show("Test");
           // this.Close();

        }
        public  int CountSubstring( string text, string value)
        {
            int count = 0, minIndex = text.IndexOf(value, 0);
            while (minIndex != -1)
            {
                minIndex = text.IndexOf(value, minIndex + value.Length);
                count++;
            }
            return count;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.SelectedPath = txInputPath.Text;
            fb.ShowNewFolderButton = false;

            if(fb.ShowDialog()==DialogResult.OK)
            {
                txInputPath.Text = fb.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.ShowNewFolderButton = true;
            fb.SelectedPath = txOutpuPath.Text;

            if (fb.ShowDialog() == DialogResult.OK)
            {
                txOutpuPath.Text = fb.SelectedPath;
            }
        }
    }
}
