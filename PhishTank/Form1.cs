using MetroFramework;
using MetroFramework.Forms;
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
using System.Xml;
using System.Xml.Linq;

namespace PhishTank
{
    public partial class Form1 : MetroForm
    {
        private bool Finished;
        private string querytext;
        private DataSet ds = new DataSet();
        private string TempxmlFile;
        private string XmlFile;
        public string AppKey = "3ea0e616372879fa683c4d7bc28f7e7cb7b73a0b530a7bb286a4fffad398cf9f"; //Phish tank's Api Appkey change to your own if you like
        public XDocument DOC;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //backgroundWorker1.Dispose();

            TempxmlFile = Application.StartupPath + @"\Meta.xml";

            XmlFile = Application.StartupPath + @"\PhishTankOut.xml";

            try
            {
                if (File.Exists(XmlFile))
                {
                    MetroMessageBox.Show(this, "Xml Detected");
                    string xml = File.ReadAllText(XmlFile);
                    xml = Cleanup_Xml.ReplaceTextInFile(xml);

                    XDocument Doc = XDocument.Parse(xml);
                    DOC = Doc;
                    Finished = true;
                }
                else
                {
                    MetroMessageBox.Show(this, "No File Detected");
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.ToString());
                MetroMessageBox.Show(this, "Error Opening Xml File");
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds.Clear();
            string text = metroTextBox1.Text;
            if (text.Length <= 3)
            {
                MetroMessageBox.Show(this, "Search string must be greater than 3 ");
                return;
            }

            try
            {
                var query = DOC.Descendants("entry").Where(f => f.Element("url").Value.Contains(metroTextBox1.Text.Trim
                    ()));

                foreach (XElement result in query)

                {
                    querytext += Environment.NewLine + Environment.NewLine + result;
                }

                string meta = querytext;

                File.WriteAllText(TempxmlFile, meta);
                querytext = "";

                XmlDocument xml = new XmlDocument();
            }
            catch

            {
                MetroMessageBox.Show(this, "Invalid Xml");
                backgroundWorker1.RunWorkerAsync();
            }

            char[] buffer = new char[10000];

            string renamedFile = TempxmlFile + ".orig";
            try
            {
                File.Move(TempxmlFile, renamedFile);
            }
            catch (FileNotFoundException)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            using (StreamReader sr = new StreamReader(renamedFile))
            using (StreamWriter sw = new StreamWriter(TempxmlFile, false))
            {
                sw.Write("<entries>");

                int read;
                while ((read = sr.Read(buffer, 0, buffer.Length)) > 0)
                    sw.Write(buffer, 0, read);

                sw.Write("</entries>");
            }
            File.Delete(renamedFile);

            ds.ReadXml(TempxmlFile);

            try
            {
                metroGrid1.DataSource = null;
                metroGrid1.Rows.Clear();

                metroGrid1.DataSource = ds.Tables[0];
            }
            catch (IndexOutOfRangeException d)
            {
                MetroMessageBox.Show(this, "Not in database");
                ///  MetroMessageBox.Show(this, d.ToString());
            }
            catch
            {
                MetroMessageBox.Show(this, "Invalid Xml");
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void metroGrid1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void ButtonDownloadDatabase_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DialogResult result = MetroMessageBox.Show(this, "Do you want to update", "Update ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DownloadDatabase Download = new DownloadDatabase();
                Download.OnFailed += Download_OnFailed;
                Download.OnFinished += Download_OnFinished;
                Download.DownloadData(AppKey, XmlFile, XmlFile);
            }
            //  MessageBox.Show("Updating Database");
            //  metroButton1.Enabled = false;
        }

        private void Download_OnFinished()
        {
            if (Finished == true)
            {
                MetroMessageBox.Show(this, "Update successful: Restart application to apply update ");
                metroButton1.Enabled = true;
            }
            else
            {
                string xml = File.ReadAllText(XmlFile);
                xml = Cleanup_Xml.ReplaceTextInFile(xml);

                XDocument Doc = XDocument.Parse(xml);
                DOC = Doc;
                Finished = true;
            }
        }

        private void Download_OnFailed()
        {
            //  MetroMessageBox.Show("PhishTank");
            MetroMessageBox.Show(this, "Database Update Failed");
            metroButton1.Enabled = true;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //  metroButton1.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker1.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Tick += Timer1_Tick;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                Spinner1.Visible = false;
                ButtonDownloadDatabase.Enabled = true;
                metroButton1.Enabled = true;
            }
            else
            {
                Spinner1.Visible = true;
                ButtonDownloadDatabase.Enabled = false;
                metroButton1.Enabled = false;
            }
        }

        private void metroButtonColor_Click(object sender, EventArgs e)
        {
            var m = new Random();
            int next = m.Next(0, 13);
            metroStyleManager1.Style = (MetroColorStyle)next;
            metroStyleManager2.Style = (MetroColorStyle)next;
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            metroGrid1.DataSource = null;
            metroGrid1.Rows.Clear();
        }

        private void metroButton2_Click_1(object sender, EventArgs e)
        {
            metroGrid1.DataSource = null;
            metroGrid1.Rows.Clear();
        }
    }
}