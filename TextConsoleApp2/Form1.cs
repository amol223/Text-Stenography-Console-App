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
using System.Configuration;
using System.Data.OleDb;
using System.IO;

namespace TextConsole
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            list = encdec.readExcel();
            foreach (var entry in list)
            {
                reverselist.Add(entry.Value, entry.Key);
            }            
        }

        Dictionary<string, string> list = new Dictionary<string, string>();
        Dictionary<string, string> reverselist = new Dictionary<string, string>();
        Dictionary<string, string> combinedlist = new Dictionary<string, string>();
        EncryptDecryptClass encdec = new EncryptDecryptClass();
        Label l = new Label();
        List<string> chars = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox5.Text = "";
            l.Text = "";
            string input = textBox1.Text;
            if (input != string.Empty)
            {
                string encryptedString = encdec.EncryptInput(input);
                StringBuilder str = new StringBuilder(encryptedString);
                textBox3.Text = encryptedString;
                string inp = "Enter a sentence with following words appearing in the given ordered sequence: ";
                for (int i = 0, j = 0; i < encryptedString.Length; i++, j++)
                {
                    if (list.ContainsKey(str[i].ToString()))
                    {
                        inp = inp + list[str[i].ToString()] + " ";
                        chars.Add("C");
                    }
                    else
                    {
                        chars.Add(str[i].ToString());
                    }                        
                }
                //inp = inp.Substring(0, inp.Length - 2);
                textBox5.Text = inp;
            }
            else
            {
                textBox3.Text = "Error: Please enter an input";
            }
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    textBox4.Text = "";
        //    string input = textBox2.Text;
        //    if (input != string.Empty)
        //    {
        //        string stringToDecodeFromSentence = encdec.decodstenograph(input, l.Text);
        //        string output = encdec.DecryptInput(stringToDecodeFromSentence);
        //        textBox4.Text = output;
        //    }
        //    else
        //    {
        //        textBox4.Text = "Error: Please enter stenograph text input";
        //    }
        //}        

        private void button2_Click(object sender, EventArgs e)
        {            
            string input = textBox2.Text;
            if (input != string.Empty)
            { 
                string output = string.Empty;
                string[] arr = input.Split(' ');             
                for (int i = 0; i < arr.Length; i++)
                {
                    if (reverselist.ContainsKey(arr[i]))
                        output = output + reverselist[arr[i]];                       
                }
                StringBuilder str = new StringBuilder(output);
                string strToDecrypt = output;
                for (int i = 0, j = 0; i < chars.Count; i++)
                {
                    if (chars[i] != "C") 
                    {
                        strToDecrypt = strToDecrypt.Substring(0, i) + chars[i] + strToDecrypt.Substring(i, strToDecrypt.Length - i);                    
                    }
                    //else
                    //{
                    //    strToDecrypt = strToDecrypt + str[j].ToString();
                    //    j++;
                    //}
                }
                string remainingString = string.Empty;

                if (ConfigurationManager.AppSettings[strToDecrypt] != null)
                {
                    remainingString = ConfigurationManager.AppSettings[strToDecrypt.ToString()];
                    Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    configuration.AppSettings.Settings.Remove(strToDecrypt);
                }
                else
                {
                    remainingString = "";
                }
                string decryptText = encdec.DecryptInput(strToDecrypt+remainingString);
                textBox4.Text = decryptText;
            }
            else
            {
                textBox4.Text = "Error: Please enter cover text input";
            }
        }        

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;            
            textBox5.Text = string.Empty;
        }    
    }
}
//The  advantage of  steganography  over  cryptography alone  is that  the intended secret  message does  not  attract attention  to itself as  an  object of scrutiny. Plainly  visible encrypted messages, no  matter  how unbreakable  they  are, arouse interest  and may  in  themselves  be incriminating in countries in which encryption is illegal. 