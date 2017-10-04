using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EncryptDecrypt;

namespace TextConsole
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        EncryptDecryptClass encdec = new EncryptDecryptClass();

        private void button1_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;
            string encryptedString = encdec.EncryptInput(input);
            textBox3.Text = encryptedString;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string input = textBox2.Text;
            string stringToDecodeFromSentence = encdec.decodstenograph(input);
            string output = encdec.DecryptInput(stringToDecodeFromSentence);
            textBox4.Text = output;
        }      

        

        
    }
}
