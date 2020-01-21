using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Drawing.Text;
using System.Linq;
using System.Management;
using System.IO.Ports;
using ZXing;

namespace NPI_printer_test
{
    class Printer
    {
        public String filePath = "";
        public String printerType = "";
        public String printerName = "";
        private StreamReader streamToPrint;
        private Font lineStyle;
        private Font fixedSys;
        static String textAling = "L";
        static float fontSize = 0;
        static String CodeQR = "";
        static int QRWidth = 100;
        static int QRHeight = 100;
        BarcodeFormat codeFormat = BarcodeFormat.PDF_417;
        public string[] getPrinters()
        {
            int index = 0;
            string[] printers = new string[PrinterSettings.InstalledPrinters.Count];
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printers[index] = printer;
                index++;
            }
            return printers;
        }
        public void TestPrinter()
        {
            var server = new LocalPrintServer();
            PrintQueue queue = server.GetPrintQueue(printerName, new string[0] { });
            bool error = false;
            Console.Out.WriteLine("{");
            Console.Out.WriteLine($"\"name\": \"{printerName}\",");
            //Load queue for correct printer
            string[] status = new string[6];
            //Check some properties of printQueue
            if (queue.IsNotAvailable)
            {
                status[0] = "\"off\"";
            }
            if (queue.IsDoorOpened)
            {
                status[1] = "\"Door\"";
            }
            if (queue.IsManualFeedRequired)
            {
                status[2] = "\"User\"";
            }
            if (queue.IsOutOfPaper)
            {
                status[3] = "\"PaperOut\"";
            }
            if (queue.IsOffline)
            {
                status[4] = "\"Offline\"";
            }
            if (queue.IsInError)
            {
                status[5] = "\"Error\"";
            }
            Console.Out.Write("\"status\":[");
            for (int i = 0; i < status.Length; i++)
            {
                if (status[i] != null)
                {
                    if (error) Console.Out.Write(",");
                    Console.Out.Write(status[i]);
                    error = true;
                }
            }
            Console.Out.WriteLine("]\n}");
        }
        public void Printing()
        {
            try
            {
                using (streamToPrint = new StreamReader(filePath))
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrinterSettings.PrinterName = printerName;
                    pd.PrinterSettings = settings(pd);
                    pd.PrintPage += new PrintPageEventHandler(printPage);
                    // Print the document.
                    pd.Print();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private PrinterSettings settings(PrintDocument printDocument)
        {
            PrinterSettings printersettings = new PrinterSettings { PrinterName = printerName };
            PrinterSettings.PaperSizeCollection psc = printersettings.PaperSizes;
            if (printerType == "NPI")
            {
                int height = getHeight(printDocument);
                PaperSize papersize = new PaperSize();
                papersize.PaperName = "Roll paper holder";
                papersize.Width = 300;
                papersize.Height = height;
                printersettings.DefaultPageSettings.PaperSize = papersize;
            }
            else
            {
                foreach (PaperSize ps in psc)
                {
                    if (ps.PaperName.Contains("80 x 3276"))
                    {
                        printersettings.DefaultPageSettings.PaperSize = ps;
                    }
                }
            }
            return printersettings;
        }
        private String addStyles(String line)
        {
            FontStyle fontStyle = FontStyle.Regular;
            String[] data = line.Split('|');
            fontSize = float.Parse(data[0]);
            textAling = data[2];
            Font barcodeFont = null;
            switch (data[1])
            {
                case "B":
                    fontStyle = FontStyle.Bold;
                    break;
                case "I":
                    fontStyle = FontStyle.Italic;
                    break;
                case "S":
                    fontStyle = FontStyle.Strikeout;
                    break;
                case "U":
                    fontStyle = FontStyle.Underline;
                    break;
                case "Bc":
                    if (data[3] != "BARCODE")
                    {
                        string actualPath = Path.GetFullPath("./");
                        PrivateFontCollection collection = new PrivateFontCollection();
                        collection.AddFontFile(actualPath + "\\resources\\dist\\electron\\static\\execute\\IDAutomationHC39M.ttf");
                        data[3] = "*" + data[3] + "*";
                        barcodeFont = new Font(collection.Families[0], fontSize);
                    }
                    else data[3] = "";
                    break;
            }
            if (barcodeFont != null)
            {
                lineStyle = barcodeFont;
            }
            else if (fontStyle == FontStyle.Bold)
            {
                string actualPath = Path.GetFullPath("./");
                PrivateFontCollection boldCollection = new PrivateFontCollection();
                boldCollection.AddFontFile(actualPath + "\\resources\\dist\\electron\\static\\execute\\fontBold.ttf");
                fixedSys = new Font(boldCollection.Families[0], fontSize);
                lineStyle = fixedSys;
            }
            else
            {
                string actualPath = Path.GetFullPath("./");
                PrivateFontCollection collection = new PrivateFontCollection();
                collection.AddFontFile(actualPath + "\\resources\\dist\\electron\\static\\execute\\font.ttf");
                fixedSys = new Font(collection.Families[0], fontSize);
                lineStyle = fixedSys;
            }
            return data[3];
        }
        public int getHeight(PrintDocument doc)
        {
            string line;
            float totalHeight = 0;
            var streamReader = new StreamReader(filePath);
            int maxHeight = 10000;
            Graphics page = Graphics.FromImage(new Bitmap(doc.PrinterSettings.DefaultPageSettings.PaperSize.Width, maxHeight));
            while ((line = streamReader.ReadLine()) != null)
            {
                String[] subLine = line.Split('/');
                if (subLine[0].Equals("pdf417") || subLine[0].Equals("Qr"))
                {
                    var codeHeight = 180;
                    try
                    {
                        codeHeight = Int32.Parse(subLine[2]);
                    }
                    catch (System.Exception)
                    {
                        codeHeight = 180;
                    }
                    totalHeight += codeHeight > 180 ? 140 : codeHeight;
                }
                else if (line[0].ToString() != "#")
                {
                    addStyles(line);
                    SizeF size = page.MeasureString(line, lineStyle);
                    totalHeight += size.Height;
                }
            }
            Console.WriteLine("totalHeight = " + totalHeight);
            return (int)totalHeight;
        }
        private void printPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = -3;
            int count = 0;
            float leftMargin;
            string line;
            // Calculate the number of lines per page.
            //linesPerPage = ev.MarginBounds.Height /
            //   printFont.GetHeight(ev.Graphics);
            // Print each line of the file.
            while ((line = streamToPrint.ReadLine()) != null)
            {
                float lastFontSize = 0;
                leftMargin = 0;
                String[] subLine = line.Split('/');
                String isComment = line[0].ToString();
                if (subLine[0].Equals("pdf417") || subLine[0].Equals("Qr")) lineStyle = fixedSys;
                else line = addStyles(line);
                if (isComment != "#")
                {
                    SizeF size = new SizeF();
                    size = ev.Graphics.MeasureString(line, lineStyle);
                    yPos += (size.Height - 1);
                    if (fontSize > 15) yPos -= 2;
                    if (fontSize > 10) yPos -= 3;
                    if (line != " ")
                    {
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.LineAlignment = StringAlignment.Center;
                        if (textAling == "C")
                        {
                            leftMargin = 140;
                            stringFormat.Alignment = StringAlignment.Center;
                        }
                        else if (textAling == "R")
                        {
                            leftMargin = 278;
                            stringFormat.Alignment = StringAlignment.Far;
                        }
                        if (subLine[0].Equals("pdf417") || subLine[0].Equals("Qr"))
                        {
                            if (subLine[0].Equals("pdf417")) codeFormat = BarcodeFormat.PDF_417;
                            else if (subLine[0].Equals("Qr")) codeFormat = BarcodeFormat.QR_CODE;
                            int confLength = subLine[0].Length + subLine[1].Length + subLine[2].Length + 3;
                            CodeQR = line.Substring(confLength, line.Length - confLength);
                            if (CodeQR != "" && CodeQR != "PDFCODE")
                            {
                                try
                                {
                                    QRWidth = Int32.Parse(subLine[1]);
                                    QRHeight = Int32.Parse(subLine[2]);
                                }
                                catch (System.Exception)
                                {
                                    Console.Write("Default size selected");
                                    if (codeFormat == BarcodeFormat.PDF_417)
                                    {
                                        QRWidth = 280;
                                        QRHeight = 200;
                                    }
                                    else
                                    {
                                        QRWidth = 200;
                                        QRHeight = 200;
                                    }
                                }
                                BarcodeWriter writer = new BarcodeWriter();
                                writer.Format = codeFormat;
                                writer.Options.Width = QRWidth;
                                writer.Options.Height = QRHeight;
                                writer.Options.Margin = 0;
                                var imgBitmap = writer.Write(CodeQR);
                                int rectHeight = imgBitmap.Height > 180 ? 140 : imgBitmap.Height;
                                int rectWidth = imgBitmap.Width > 280 ? 280 : imgBitmap.Width;
                                Rectangle rect = new Rectangle(140 - (rectWidth / 2), (int)yPos, rectWidth, rectHeight);
                                ev.Graphics.DrawImage(imgBitmap, rect);
                                yPos += rectHeight;
                            }
                        }
                        else if (size.Width >= 292)
                        {
                            int lineSize = line.Length;
                            int linesNumber = (int)(size.Width / 280) + 1;
                            Rectangle rect = new Rectangle(0, (int)(yPos -= fontSize / linesNumber), 280, (int)(size.Height * linesNumber));
                            stringFormat.LineAlignment = StringAlignment.Far;
                            ev.Graphics.DrawString(line, lineStyle, Brushes.Black, rect, stringFormat);
                            //ev.Graphics.DrawRectangle(new Pen(Brushes.Black, 1), rect);
                            yPos += rect.Height - (fontSize / linesNumber);
                        }
                        else ev.Graphics.DrawString(line, lineStyle, Brushes.Black, leftMargin, yPos, stringFormat);
                        count++;
                        lastFontSize = fontSize;
                    }
                    else
                    {
                        linesPerPage -= 1;
                    }
                    Console.WriteLine("height ypos: " + yPos);
                }
                else Console.Write("commented line");
            }
        }
    }
}
