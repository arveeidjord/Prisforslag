using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;
using System.Threading;
using System.IO;

namespace Corinor.DataAksess
{
    public class UpdatePrisliste2
    {
        public event EventHandler PrislisteDownloded;
        public event EventHandler PrislisteDownloadError;

        static Uri uri = null;

        public void GetPrislisteAsync(string updateUrl)
        {
            uri = new Uri(updateUrl);

            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork +=new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            bg.RunWorkerAsync();
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                PrislisteDownloadError(e.Error, null);
                return;
            }
            
            PrislisteDownloded(e.Result, null);
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            var tempfilUrl = Path.GetTempFileName();

            var webClient = new WebClient();
            webClient.DownloadFile(uri, tempfilUrl);
            webClient.Dispose();

            e.Result = tempfilUrl;
        }
    }
}
