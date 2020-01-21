namespace NPI_printer_test
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.printerSelector = new System.Windows.Forms.ComboBox();
            this.printBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // printerSelector
            // 
            this.printerSelector.FormattingEnabled = true;
            this.printerSelector.Location = new System.Drawing.Point(47, 116);
            this.printerSelector.Name = "printerSelector";
            this.printerSelector.Size = new System.Drawing.Size(193, 21);
            this.printerSelector.TabIndex = 0;
            this.printerSelector.SelectedIndexChanged += new System.EventHandler(this.printerSelector_SelectedIndexChanged);
            // 
            // printBtn
            // 
            this.printBtn.Location = new System.Drawing.Point(100, 148);
            this.printBtn.Name = "printBtn";
            this.printBtn.Size = new System.Drawing.Size(75, 23);
            this.printBtn.TabIndex = 1;
            this.printBtn.Text = "Print";
            this.printBtn.UseVisualStyleBackColor = true;
            this.printBtn.Click += new System.EventHandler(this.printBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 241);
            this.Controls.Add(this.printBtn);
            this.Controls.Add(this.printerSelector);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox printerSelector;
        private System.Windows.Forms.Button printBtn;
    }
}

