using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;

namespace Corinor.Hjelpeklasser
{
    //Feilkoder for MAPI:
    //Global Const SUCCESS_SUCCESS = 0
    //Global Const MAPI_USER_ABORT = 1
    //Global Const MAPI_E_USER_ABORT = MAPI_USER_ABORT
    //Global Const MAPI_E_FAILURE = 2
    //Global Const MAPI_E_LOGIN_FAILURE = 3
    //Global Const MAPI_E_LOGON_FAILURE = MAPI_E_LOGIN_FAILURE
    //Global Const MAPI_E_DISK_FULL = 4
    //Global Const MAPI_E_INSUFFICIENT_MEMORY = 5
    //Global Const MAPI_E_BLK_TOO_SMALL = 6
    //Global Const MAPI_E_TOO_MANY_SESSIONS = 8
    //Global Const MAPI_E_TOO_MANY_FILES = 9
    //Global Const MAPI_E_TOO_MANY_RECIPIENTS = 10
    //Global Const MAPI_E_ATTACHMENT_NOT_FOUND = 11
    //Global Const MAPI_E_ATTACHMENT_OPEN_FAILURE = 12
    //Global Const MAPI_E_ATTACHMENT_WRITE_FAILURE = 13
    //Global Const MAPI_E_UNKNOWN_RECIPIENT = 14
    //Global Const MAPI_E_BAD_RECIPTYPE = 15
    //Global Const MAPI_E_NO_MESSAGES = 16
    //Global Const MAPI_E_INVALID_MESSAGE = 17
    //Global Const MAPI_E_TEXT_TOO_LARGE = 18
    //Global Const MAPI_E_INVALID_SESSION = 19
    //Global Const MAPI_E_TYPE_NOT_SUPPORTED = 20
    //Global Const MAPI_E_AMBIGUOUS_RECIPIENT = 21
    //Global Const MAPI_E_AMBIG_RECIP = MAPI_E_AMBIGUOUS_RECIPIENT
    //Global Const MAPI_E_MESSAGE_IN_USE = 22
    //Global Const MAPI_E_NETWORK_FAILURE = 23
    //Global Const MAPI_E_INVALID_EDITFIELDS = 24
    //Global Const MAPI_E_INVALID_RECIPS = 25
    //Global Const MAPI_E_NOT_SUPPORTED = 26


    //http://stackoverflow.com/questions/587136/mapisendmail-doesnt-work-when-outlook-is-running //Brukes
    //http://www.codeproject.com/KB/IP/SendFileToNET.aspx
    //http://weblogs.asp.net/jgalloway/archive/2007/02/24/sending-files-via-the-default-e-mail-client.aspx
    class EpostSender
    {

        public static bool SendEpost(string vedleggUrl, string emne, string sendTil) 
        {
            int res = SendMail(vedleggUrl, emne, sendTil);
            if (res == 0 || res == 1) return true;
            else
            {
                MessageBox.Show("Klarte ikke å åpne e-postvindu for sending.\n\nFeilmedling med kode: " + res, "Corinor prisforslag", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        
        }

        
            [DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
            public static extern int MAPISendMail(IntPtr lhSession, IntPtr ulUIParam,
                MapiMessage lpMessage, int flFlags, int ulReserved);
            public const int MAPI_LOGON_UI = 0x00000001;
            private const int MAPI_DIALOG = 0x00000008;

            static int SendMail(string strAttachmentFileName, string strSubject, string to)
            {
                if (string.IsNullOrEmpty(to))
                    to = " ";
                try
                {
                    MapiMessage msg = new MapiMessage();
                    msg.subject = strSubject;

                    int sizeofMapiDesc = Marshal.SizeOf(typeof(MapiFileDesc));
                    IntPtr pMapiDesc = Marshal.AllocHGlobal(sizeofMapiDesc);

                    MapiFileDesc fileDesc = new MapiFileDesc();
                    fileDesc.position = -1;
                    int ptr = (int)pMapiDesc;

                    string path = strAttachmentFileName;
                    fileDesc.name = Path.GetFileName(path);
                    fileDesc.path = path;
                    Marshal.StructureToPtr(fileDesc, (IntPtr)ptr, false);

                    msg.files = pMapiDesc;
                    msg.fileCount = 1;

                    List<MapiRecipDesc> recipsList = new List<MapiRecipDesc>();

                    MapiRecipDesc recipient = new MapiRecipDesc();

                    recipient.recipClass = 1;
                    recipient.name = to;
                    recipsList.Add(recipient);
                    

                    int size = Marshal.SizeOf(typeof(MapiRecipDesc));
                    IntPtr intPtr = Marshal.AllocHGlobal(recipsList.Count * size);

                    int recipPtr = (int)intPtr;
                    foreach (MapiRecipDesc mapiDesc in recipsList)
                    {
                        Marshal.StructureToPtr(mapiDesc, (IntPtr)recipPtr, false);
                        recipPtr += size;
                    }

                    msg.recips = intPtr;
                    msg.recipCount = 1;
                    int result = MAPISendMail(new IntPtr(0), new IntPtr(0), msg, MAPI_LOGON_UI | MAPI_DIALOG, 0);

                    return result;
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }

                return -1;
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
