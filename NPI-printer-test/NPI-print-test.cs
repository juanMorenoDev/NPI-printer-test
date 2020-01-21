using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NPI_printer_test
{
    public partial class Form1 : Form
    {
        Printer printer = new Printer();
        String selectedPrinter;
        public Form1()
        {
            InitializeComponent();
            FillPrinterSelector();
        }

        private void printerSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPrinter = printerSelector.Text;
        }

        private void FillPrinterSelector()
        {
            foreach(string printerName in printer.getPrinters())
            {
                printerSelector.Items.Add(printerName);
            }
        }

        private void printBtn_Click(object sender, EventArgs e)
        {
            printer.printerName = selectedPrinter;
            printer.printerType = "NPI";
            printer.filePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Ticket1.txt";
            printer.Printing();
            printer.filePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Ticket2.txt";
            printer.Printing();
            printer.filePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Ticket3.txt";
            printer.Printing();
        }
    }
}
