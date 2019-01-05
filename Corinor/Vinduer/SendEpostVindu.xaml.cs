using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Corinor.Vinduer
{
    /// <summary>
    /// Interaction logic for SendEpostVindu.xaml
    /// </summary>
    public partial class SendEpostVindu : Window
    {

        public SendEpostVindu()
        {
            InitializeComponent();
            this.avbrytKnapp.Click += new RoutedEventHandler(avbrytKnapp_Click);
            this.sendKnapp.Click += new RoutedEventHandler(sendKnapp_Click);
        }

        void sendKnapp_Click(object sender, RoutedEventArgs e)
        {
            sendKnapp.IsEnabled = false;
            sendKnapp.Content = "Sender...";
            if (!sendEpost("Prisforslag", "d:\\file.txt", "fullpostkasse@gmail.com"))
            {
                MessageBox.Show("Klarte ikke å sende e-posten. \nSjekk om du er koblet til Internett.");

                sendKnapp.Content = "Send";
                sendKnapp.IsEnabled = true;


                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        void avbrytKnapp_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private bool sendEpost(string subjekt, string vedleggUrl, string sendTil)
        {
            return false;
            //int res = Hjelpeklasser.EpostSender.SendMail(vedleggUrl, subjekt, sendTil);
            //if (res == 0) return true;
            //else return false;
        }


        private bool sendEpost2()
        {

            //string to = tilTekstboks.Text;
            //string from = "fullpostkasse@gmail.com";
            //string subject = emneTekstboks.Text;
            //string body = kommentarTekstboks.Text;
            //MailMessage message = new MailMessage(from, to, subject, body);
            //SmtpClient client = new SmtpClient(smtpTekstboks.Text);
            //client.Port = 465;
            ////Console.WriteLine("Changing time out from {0} to 100.", client.Timeout);
            //client.Timeout = 5000;
            //// Credentials are necessary if the server requires the client 
            //// to authenticate before it will send e-mail on the client's behalf.
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;
            
            //try
            //{
            //    client.Send(message);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Exception caught in CreateTimeoutTestMessage(): {0}",
            //          ex.ToString());
            //}




            //try
            //{
            //    MailMessage mail = new MailMessage();
            //    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            //    mail.From = new MailAddress("your_email_address@gmail.com");
            //    mail.To.Add("fullpostkasse@gmail.com");
            //    mail.Subject = "Test Mail";
            //    mail.Body = "This is for testing SMTP mail from GMAIL";

            //    SmtpServer.Port = 587;
            //    SmtpServer.Credentials = CredentialCache.DefaultNetworkCredentials; //new System.Net.NetworkCredential("fullpostkasse@gmail.com", "1Bug4You");
            //    SmtpServer.EnableSsl = true;

            //    SmtpServer.Send(mail);
            //    MessageBox.Show("mail Send");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
            //string filename = "mailto:jismenon@yahoo.com?subject=Hello&boby=Hello&cc=meera@yahoo.com&bcc=jany@yahoo.com&Attach=\"d:\\test.txt\"";
            //filename = "mailto:user@example.com?subject=Message Title&body=Message Content&Attachment=\"d:\\test.txt\"";
            //Process.Start(filename);

            //"mailto:user@example.com?subject=Message Title&body=Message Content&Attachment=\"d:\\test.txt\""



            return true;
        }

        public static void CreateTimeoutTestMessage(string server)
        {
            string to = "jane@contoso.com";
            string from = "ben@contoso.com";
            string subject = "Using the new SMTP client.";
            string body = @"Using this new feature, you can send an e-mail message from an application very easily.";
            MailMessage message = new MailMessage(from, to, subject, body);
            SmtpClient client = new SmtpClient(server);
            Console.WriteLine("Changing time out from {0} to 100.", client.Timeout);
            client.Timeout = 100;
            // Credentials are necessary if the server requires the client 
            // to authenticate before it will send e-mail on the client's behalf.
            client.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTimeoutTestMessage(): {0}",
                      ex.ToString());
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void expander1_Expanded(object sender, RoutedEventArgs e)
        {

        }


    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MapiMessage
    {
        public int reserved;
        public string subject;
        public string noteText;
        public string messageType;
        public string dateReceived;
        public string conversationID;
        public int flags;
        public IntPtr originator;
        public int recipCount;
        public IntPtr recips;
        public int fileCount;
        public IntPtr files;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MapiFileDesc
    {
        public int reserved;
        public int flags;
        public int position;
        public string path;
        public string name;
        public IntPtr type;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class MapiRecipDesc
    {
        public int reserved;
        public int recipClass;
        public string name;
        public string address;
        public int eIDSize;
        public IntPtr entryID;
    }
}
