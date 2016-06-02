using MetroFramework;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

internal class DownloadDatabase
{
    public delegate void Finished();

    public delegate void Failed();

    public event Finished OnFinished;

    public event Failed OnFailed;

    public bool status = false;

    public void DownloadData(string Appkey, string directoryin, string directoryout)
    {
        try
        {
            using (WebClient myWebClient = new WebClient())
            {
                ServicePointManager.DefaultConnectionLimit = 30;

                // Download the Web resource and save it into the current filesystem folder.
                String Url_Appkey = "http://data.phishtank.com/data/" + Appkey + "/online-valid.xml";
                myWebClient.DownloadFile(Url_Appkey, directoryin);
                string xmlText = File.ReadAllText(directoryin);
                //   xmlText = Cleanup_Xml.ReplaceTextInFile(xmlText);
                File.WriteAllText(directoryout, xmlText);
                status = true;

                if (OnFinished != null)
                {
                    OnFinished();
                }
            }
        }
        catch (WebException ex)
        {
            status = false;

            MessageBox.Show(ex.ToString());
            if (OnFailed != null)
            {
                OnFailed();
            }
        }
    }
}