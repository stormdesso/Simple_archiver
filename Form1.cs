using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab6_v2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Dialog(TextBox field)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.

            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                field.Text = fileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)//выбор 1-го файла
        {
            Dialog(textBox1);
        }

        private void button2_Click(object sender, EventArgs e)//выбор 2-го файла
        {
            Dialog(textBox2);
        }

        private void button5_Click(object sender, EventArgs e)//Stop
        {
            Archivator.Stop(true);   
            progressBar1.Value = 0;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
       
        private void button3_Click(object sender, EventArgs e)//Pack
        {
            Archivator arch = new Archivator();
            arch.notifier += progressBarUpdate;
            arch.Pack(textBox1.Text, textBox2.Text);            
        }

        private void button4_Click(object sender, EventArgs e)//Unpack
        {
            Archivator arch = new Archivator();
            arch.notifier += progressBarUpdate;
            arch.Unpack(textBox1.Text, textBox2.Text);           
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
              
        }
        private void progressBarUpdate(int progress)
        {
            progressBar1.Value = progress;
            Application.DoEvents();            
        }
    }
}
