using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows;
using System.Printing;
using System.Windows.Xps;
using System.Windows.Documents.Serialization;
using System.Windows.Xps.Packaging;
using System.IO;
//using PdfSharp.Pdf;

namespace Corinor.Kontrollbehandling
{
    public class Printer
    {
        //http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/4ba15a81-d8a7-4efe-974f-beebf832bdb3

        public static string PrintSom(FrameworkElement kundeInfo, ScrollViewer scrollViewer, bool lagreSomPDF, string pdfUrl, GridViewColumn c1SomSkalSkjules, GridViewColumn c2SomSkalSkjules, bool liggende)
        {
            string utUrl = "";
            double c1w = -1;
            double c2w = -1;
    
            //#Skjule kolonner
            if (c1SomSkalSkjules != null)
            {
                c1w = c1SomSkalSkjules.Width;
                c1SomSkalSkjules.Width = 0;
            }

            if (c2SomSkalSkjules != null)
            {
                c2w = c2SomSkalSkjules.Width;
                c2SomSkalSkjules.Width = 0;
            }
            //###

            try
            {
                if (lagreSomPDF)
                    utUrl = PrintToPDF(kundeInfo, scrollViewer, pdfUrl, liggende);
                else
                    Print(kundeInfo, scrollViewer);
            }
            catch (Exception)
            {
                utUrl = "";
            }
           


            //Vise kolonner
            if (c1SomSkalSkjules != null)
                c1SomSkalSkjules.Width = c1w;

            if (c2SomSkalSkjules != null)
                c2SomSkalSkjules.Width = c2w;
            //###

            return utUrl;
        }



        private static void Print(FrameworkElement kundeInfo, ScrollViewer scrollViewer) 
	    {
            if (kundeInfo == null && scrollViewer == null) return;

            PrintDocumentImageableArea area = null; 
	        PageRangeSelection selection = PageRangeSelection.AllPages; 
	        PageRange range = new PageRange(); 
	        XpsDocumentWriter xpsdw1 = PrintQueue.CreateXpsDocumentWriter("Corinor prisforslag", ref area, ref selection, ref range);
            
            if (xpsdw1 == null) return;

            //TODO: DEBUG
            //if (File.Exists("D:\\test.xps")) File.Delete("D:\\test.xps");
            //XpsDocument _xpsDocument = new XpsDocument("D:\\test.xps", FileAccess.ReadWrite);
            //XpsDocumentWriter xpsdw = XpsDocument.CreateXpsDocumentWriter(_xpsDocument);


            Thickness Margins = new Thickness(96);

            double leftMargin = Margins.Left - area.OriginWidth;
            double topMargin = Margins.Top - area.OriginHeight;
            double rightMargin = Margins.Right - (area.MediaSizeWidth - area.ExtentWidth - area.OriginWidth);
            double bottomMargin = Margins.Bottom - (area.MediaSizeHeight - area.ExtentHeight - area.OriginHeight);
            Size outputSize = new Size(
                area.MediaSizeWidth - leftMargin - rightMargin,
                area.MediaSizeHeight - topMargin - bottomMargin);

            SerializerWriterCollator batchPrinter = xpsdw1.CreateVisualsCollator();
            batchPrinter.BeginBatchWrite();

            if (kundeInfo != null) printKundeinfo(batchPrinter, kundeInfo, outputSize, leftMargin, topMargin);
            if (scrollViewer != null && scrollViewer.Content != null) printScrollViewer(batchPrinter, scrollViewer, outputSize, leftMargin, topMargin);

            batchPrinter.EndBatchWrite(); 
	 
            //TODO: Debug
            //_xpsDocument.Close();


	    }

        private static string PrintToPDF(FrameworkElement kundeInfo, ScrollViewer scrollViewer, string lagreTilUrl, bool liggende)
        {
            if (kundeInfo == null && scrollViewer == null) return "";

            string prisforslagXPSuri = Path.Combine(Hjelpeklasser.GlobaleUrier.standardMappe2(), "Prisforslag.xps");
            string prisforslagPDFuri = Path.Combine(Hjelpeklasser.GlobaleUrier.standardMappe2(), "Prisforslag.pdf");
            if (!string.IsNullOrEmpty(lagreTilUrl)) prisforslagPDFuri = lagreTilUrl;

            if (File.Exists(prisforslagXPSuri)) File.Delete(prisforslagXPSuri);
            XpsDocument _xpsDocument = new XpsDocument(prisforslagXPSuri, FileAccess.ReadWrite);
            XpsDocumentWriter xpsdw = XpsDocument.CreateXpsDocumentWriter(_xpsDocument);

            Size docSize;
            if (liggende) docSize = new Size(1122.24, 793.59874015748028);
            else docSize  = new Size(793.59874015748028, 1122.24);

            double leftMargin = 84.5; //Margins.Left - area.OriginWidth;
            double topMargin = 84.5;///Margins.Top - area.OriginHeight;
            double rightMargin = 84.5;//Margins.Right - (docSize.Width - area.ExtentWidth - area.OriginWidth);
            double bottomMargin = 84.5; //Margins.Bottom - (docSize.Height - area.ExtentHeight - area.OriginHeight);
            Size outputSize = new Size(
                docSize.Width - leftMargin - rightMargin,
                docSize.Height - topMargin - bottomMargin);


            SerializerWriterCollator batchPrinter = xpsdw.CreateVisualsCollator();
            batchPrinter.BeginBatchWrite();

            if (kundeInfo != null) printKundeinfoToPdf(batchPrinter, kundeInfo, outputSize, leftMargin, topMargin);
            if (scrollViewer != null && scrollViewer.Content != null) 
                printScrollViewer(batchPrinter, scrollViewer, outputSize, leftMargin, topMargin);

            batchPrinter.EndBatchWrite();
            
            _xpsDocument.Close();
            PdfSharp.Xps.XpsConverter.Convert(prisforslagXPSuri, prisforslagPDFuri, 0);



            return prisforslagPDFuri;
            
        }

        private static void printKundeinfoToPdf(SerializerWriterCollator batchPrinter, FrameworkElement kundeinfoKontroll, Size outputSize, double leftMargin, double topMargin)
        {
            Size elementSize = new Size(outputSize.Width + leftMargin + leftMargin, outputSize.Height + topMargin + topMargin);
            Rect rec = new Rect(new Point(leftMargin, topMargin), elementSize);

            kundeinfoKontroll.ClipToBounds = true;
            kundeinfoKontroll.Clip = new RectangleGeometry(new Rect(outputSize));
            kundeinfoKontroll.Measure(elementSize);
            kundeinfoKontroll.Arrange(rec);
            batchPrinter.Write(kundeinfoKontroll);
        }


        private static void printKundeinfo(SerializerWriterCollator batchPrinter, FrameworkElement kundeinfoKontroll, Size outputSize, double leftMargin, double topMargin)
        {
            Size elementSize = new Size(outputSize.Width, outputSize.Height);
            Rect rec = new Rect(new Point(leftMargin, topMargin), elementSize);
            kundeinfoKontroll.Arrange(rec);
            batchPrinter.Write(kundeinfoKontroll);
        }


        private static void printScrollViewer(SerializerWriterCollator batchPrinter, ScrollViewer scrollViewer, Size outputSize, double leftMargin, double topMargin)//, PrintDocumentImageableArea area)
        {
            //if (scrollViewer == null || scrollViewer.Content == null || !(scrollViewer.Content is FrameworkElement)) return;
            FrameworkElement element = scrollViewer.Content as FrameworkElement;

            //Husker properties
            bool originalClipToBounds = element.ClipToBounds;
            Geometry originalClip = element.Clip;
            Thickness originalMargin = element.Margin;
            Size elementSize = new Size(outputSize.Width, element.ActualHeight);
            Size orginalSize = new Size(element.ActualWidth, element.ActualHeight);

            element.ClipToBounds = true;
            element.Clip = new RectangleGeometry(new Rect(outputSize));
            element.Measure(elementSize);
            element.Arrange(new Rect(new Point(leftMargin, topMargin), elementSize));

            batchPrinter.Write(element);
            double currHeight = outputSize.Height;
            while (currHeight < element.ActualHeight)
            {
                elementSize.Height -= outputSize.Height;
                element.Margin = new Thickness(0, -currHeight, 0, 0);
                element.Clip = new RectangleGeometry(new Rect(new Point(0, currHeight), outputSize));
                element.Measure(elementSize);
                element.Arrange(new Rect(new Point(leftMargin, topMargin), elementSize));
                batchPrinter.Write(element);
                currHeight += outputSize.Height;
            }

            //Rydder opp
            element.ClipToBounds = originalClipToBounds;
            element.Clip = originalClip;
            element.Margin = originalMargin;
            element.Arrange(new Rect(new Point(0, 0), orginalSize)); 

        }


        //http://www.a2zdotnet.com/View.aspx?Id=66
        //http://www.eggheadcafe.com/tutorials/aspnet/9cbb4841-8677-49e9-a3a8-46031e699b2e/wpf-printing-and-print-preview.aspx
        //http://blog.saraf.me/2011/05/wpf-print-engine/ (ENGINE)
        //http://www.nbdtech.com/Blog/archive/2009/07/09/wpf-printing-part-4-ndash-print-preview.aspx
        //http://www.thomasclaudiushuber.com/blog/2009/11/24/wpf-printing-how-to-print-a-pagerange-with-wpfs-printdialog-that-means-the-user-can-select-specific-pages-and-only-these-pages-are-printed/
    }
}
