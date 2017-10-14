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
        Label l = new Label();
        
        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox5.Text = "";
            l.Text = "";
            string input = textBox1.Text;
            if (input != string.Empty)
            {
                string encryptedString = encdec.EncryptInput(input);
                textBox3.Text = encryptedString;
                int numberofchars = encryptedString.Length;
                int numberofwords = 1 + (8 * numberofchars);
                textBox5.Text = "Enter " + numberofwords.ToString() + " words here";
                l.Text = numberofwords.ToString();
                l.Hide();
            }
            else
            {
                textBox3.Text = "Error: Please enter an input";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox4.Text = "";
            string input = textBox2.Text;
            if (input != string.Empty)
            {
                string stringToDecodeFromSentence = encdec.decodstenograph(input, l.Text);
                string output = encdec.DecryptInput(stringToDecodeFromSentence);
                textBox4.Text = output;
            }
            else
            {
                textBox4.Text = "Error: Please enter stenograph text input";
            }
        }        

        private void button3_Click(object sender, EventArgs e)
        {
            textBox6.Text = "";
            string covertext = textBox5.Text;
            if (covertext != string.Empty) { 
            string output = string.Empty;
            //List<string> output = new List<string>();
            string[] arr = covertext.Split(' ');
            int numberofwords = Convert.ToInt32(l.Text);
            string hidetext = textBox3.Text;
            StringBuilder chararray = new StringBuilder(hidetext);
            int j = 0;
            int h = 0;
            if (numberofwords <= arr.Length)
            {
                //output = output + arr[j];
                //j++;
                for (int i = 0; i < chararray.Length; i++)
                {
                    string binary = Convert.ToString(chararray[i], 2).PadLeft(8, '0');
                    StringBuilder bin = new StringBuilder(binary);
                    for (int k = 0; k < bin.Length; k++)
                    {
                        if (bin[k] == '1')
                        {
                            output = output + arr[j] + " ";
                        }
                        else
                        {                           
                            output = output + arr[j] + "  ";
                        }
                        j++;
                    }
                }
                if (numberofwords == arr.Length)
                    output = output + arr[arr.Length - 1];
                else
                {
                    while (j < arr.Length)
                    {
                        output = output + arr[j] + " ";
                        j++;
                    }
                }                    
            }
            else
                output = ((numberofwords - arr.Length) > 1 ? "Error: Please enter " + (numberofwords - arr.Length) + " more words." :
                    "Error: Please enter " + (numberofwords - arr.Length) + " more word.");
            textBox6.Text = output;
            }
            else
            {
                textBox6.Text = "Error: Please enter cover text input";
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            textBox6.Text = string.Empty;
        }      

        

        
    }
}
//The  advantage of  steganography  over  cryptography alone  is that  the intended secret  message does  not  attract attention  to itself as  an  object of scrutiny. Plainly  visible encrypted messages, no  matter  how unbreakable  they  are, arouse interest  and may  in  themselves  be incriminating in countries in which encryption is illegal. 