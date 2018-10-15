using System.Windows.Forms;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System;

namespace RDotNet
{
    internal class Form1 : Form
    {

        private Button printPlot = new Button();
        private Button printData = new Button();


        public Form1()
        {
            printPlot.Text = "Print Avg and Std";
            printPlot.Click += new EventHandler(printPlot_Click);

            printPlot.Text = "Print Plot";
            printPlot.Click += new EventHandler(printData_Click);

            this.Controls.Add(printPlot);
            this.Controls.Add(printData);
        }

        void printPlot_Click(object sender, EventArgs e)
        {
            PictureBox imageControl = new PictureBox();
            imageControl.Width = 800;
            imageControl.Height = 800;
            this.Size = new Size(750, 750);
            Bitmap image = new Bitmap("myplot.png");
            imageControl.Dock = DockStyle.Fill;
            imageControl.Image = (Image)image;
            Controls.Add(imageControl);
        }
        void printData_Click(object sender, EventArgs e)
        {
            
          
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(747, 534);
            this.Name = "Form1";
            this.ResumeLayout(false);

        }
    }
}