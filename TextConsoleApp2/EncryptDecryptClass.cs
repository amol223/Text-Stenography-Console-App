using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace EncryptDecrypt
{
    public class EncryptDecryptClass //encrypt decrypt class
    {
        
        public EncryptDecryptClass()
        {
            
        }

        public string EncryptInput(string input)
        {
            string key = (ConfigurationManager.AppSettings["secretkey"]); //read encrypt key from config file
            UInt32 sumOfKeyChar = (UInt32)key.Select(x => (int)x).Sum(); //sum of ascii value of all chars in key
            //Console.WriteLine(sumOfKeyChar);
            int lengthOfKey = key.Length; //length of key
            //Console.WriteLine(lengthOfKey);
            UInt32 initialValue = GetInitValue(sumOfKeyChar, lengthOfKey); //compute initial value
            //Console.WriteLine(initialValue);
            string output = startEncryption(input, (int)initialValue); //encrypt function
            //Console.WriteLine(output);
            return output;
        }

        public string DecryptInput(string input)
        {
            string key = (ConfigurationManager.AppSettings["secretkey"]); //read encrypt key from config file
            UInt32 sumOfKeyChar = (UInt32)key.Select(x => (int)x).Sum(); //sum of ascii value of all chars in key
            //Console.WriteLine(sumOfKeyChar);
            int lengthOfKey = key.Length; //length of key
            //Console.WriteLine(lengthOfKey);
            UInt32 initialValue = GetInitValue(sumOfKeyChar, lengthOfKey); //compute initial value
            //Console.WriteLine(initialValue);
            string output = startDecryption(input, (int)initialValue); //decrypt function
            //Console.WriteLine(output);
            return output;
        }

        private UInt32 GetInitValue(UInt32 sumOfKeyChar, int shift)
        {
            string shiftedBitString = rightRotateShift(Convert.ToString(sumOfKeyChar, 2).ToString(), 7); //right circular bit shift
            //Console.WriteLine(shiftedBitString);
            UInt32 initialValue = Convert.ToUInt32(shiftedBitString, 2); //initial shift value
            //Console.WriteLine(initialValue);
            return initialValue;
        }

        private string rightRotateShift(string key, int shift) //right circular bit shift
        {
            shift %= key.Length;
            return key.Substring(key.Length - shift) + key.Substring(0, key.Length - shift);
        }

        private string startEncryption(string input, int initialValue) //encryption function
        {
            StringBuilder inputCharArray = new StringBuilder(input); //string input to array of chars
            List<string> specialCharPositions = new List<string>();  //list of special char positions          
            int offset = 0; //shift value

            //loop each char in input string for encryption
            for(int count = 0; count < inputCharArray.Length; count++)
            {
                offset = initialValue % 10; //shift value for char
                
                if (count % 2 == 0) //even place char
                {
                    if (inputCharArray[count] >= 65 && inputCharArray[count] <= 90) //uppercase alphabet validation
                    {
                        if ((inputCharArray[count] - offset) < 65) //circular shift overflow
                            inputCharArray[count] = Convert.ToChar(91 - (offset - (inputCharArray[count] - 65 )));
                        else
                            inputCharArray[count] = Convert.ToChar((inputCharArray[count] - offset));
                    }
                    else if (inputCharArray[count] >= 97 && inputCharArray[count] <= 122) //lowercase alphabet validation
                    {
                        if ((inputCharArray[count] - offset) < 97) //circular shift overflow
                            inputCharArray[count] = Convert.ToChar(123 - (offset - (inputCharArray[count] - 97)));
                        else
                            inputCharArray[count] = Convert.ToChar((inputCharArray[count] - offset));
                    }
                    else 
                    {
                        //nonalphabet positions to list
                        inputCharArray[count] = Convert.ToChar((inputCharArray[count] + offset));
                        specialCharPositions.Add(count.ToString());
                        specialCharPositions.Add("."); //split specifier
                    }
                }
                else
                {
                    if (inputCharArray[count] >= 65 && inputCharArray[count] <= 90) 
                    {
                        if ((inputCharArray[count] + offset) > 90)
                            inputCharArray[count] = Convert.ToChar(64 + (offset - (90 - inputCharArray[count])));
                        else
                            inputCharArray[count] = Convert.ToChar((inputCharArray[count] + offset));
                    }
                    else if (inputCharArray[count] >= 97 && inputCharArray[count] <= 122)
                    {
                        if ((inputCharArray[count] + offset) > 122)
                            inputCharArray[count] = Convert.ToChar(96 + (offset - (122 - inputCharArray[count])));
                        else
                            inputCharArray[count] = Convert.ToChar((inputCharArray[count] + offset));
                    }
                    else
                    {
                        
                        inputCharArray[count] = Convert.ToChar((inputCharArray[count] + offset));
                        specialCharPositions.Add(count.ToString());
                        specialCharPositions.Add(".");
                    }
                }   
             
                initialValue++;
            }

            if(specialCharPositions.Count !=  0)
                specialCharPositions.Add(specialCharPositions.Count.ToString()); //number of nonalphabetic chars
            else
                specialCharPositions.Add("."); //no nonalphabetic char case
            string specialCharString = String.Join(String.Empty, specialCharPositions.ToArray()); 
            //Console.WriteLine(specialCharString);
            inputCharArray.Append(".");
            inputCharArray.Append(specialCharString);
            //Console.WriteLine(inputCharArray.ToString());
            string encryptedString = inputCharArray.ToString(); //encrypted string
            //Console.WriteLine(encryptedString);

            string[] splitString = encryptedString.Split('.');
            StringBuilder charArray = new StringBuilder(splitString[0]);
            string remainingString = encryptedString.Substring(charArray.Length, encryptedString.Length - charArray.Length);
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings[splitString[0]] == null)
            {
                configuration.AppSettings.Settings.Add(splitString[0], remainingString);
            }
            else
            {
                configuration.AppSettings.Settings.Remove(splitString[0]);
                configuration.AppSettings.Settings.Add(splitString[0], remainingString);
            }
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings"); 
            encryptedString = splitString[0];

            return encryptedString;
        }

        private string startDecryption(string input, int initialValue) //decryption function
        {
            string[] splitString = input.Split('.'); //split char and nonalphabets
            string stringToDecrypt = splitString[0]; //actual string to decrypt
            //Console.WriteLine(stringToDecrypt);
            splitString[0] = ""; //eliminate string to decrypt
            splitString[splitString.Length - 1] = ""; //eliminate number of nonalphabets
            //string specialCharPositionString = string.Concat(splitString); //
            //Console.WriteLine(specialCharPositionString);
            
            StringBuilder inputCharArray = new StringBuilder(stringToDecrypt); //char array of string
            int offset = 0; // shift offset

            //loop for each char to decrypt
            for (int count = 0; count < inputCharArray.Length; count++)
            {
                offset = initialValue % 10; //last digit shift value

                if (count % 2 != 0) // odd place char
                {
                    if (!splitString.Contains(count.ToString())) // no nonalphabetic char
                    {
                        if (inputCharArray[count] >= 65 && inputCharArray[count] <= 90) //uppercase alphabet validation
                        {
                            if ((inputCharArray[count] - offset) < 65) //alphabet overflow
                                inputCharArray[count] = Convert.ToChar(91 - (offset - (inputCharArray[count] - 65)));
                            else
                                inputCharArray[count] = Convert.ToChar((inputCharArray[count] - offset));
                        }
                        else if (inputCharArray[count] >= 97 && inputCharArray[count] <= 122) //lowercase alphabet validation
                        {
                            if ((inputCharArray[count] - offset) < 97) //alphabet overflow
                                inputCharArray[count] = Convert.ToChar(123 - (offset - (inputCharArray[count] - 97)));
                            else
                                inputCharArray[count] = Convert.ToChar((inputCharArray[count] - offset));
                        }                        
                    }
                    else
                    {
                        //nonalphabetic char decryption
                        inputCharArray[count] = Convert.ToChar((inputCharArray[count] - offset));
                    }
                }
                else //even place char decryption
                {
                    if (!splitString.Contains(count.ToString()))
                    {
                        if (inputCharArray[count] >= 65 && inputCharArray[count] <= 90)
                        {
                            if ((inputCharArray[count] + offset) > 90)
                                inputCharArray[count] = Convert.ToChar(64 + (offset - (90 - inputCharArray[count])));
                            else
                                inputCharArray[count] = Convert.ToChar((inputCharArray[count] + offset));
                        }
                        else if (inputCharArray[count] >= 97 && inputCharArray[count] <= 122)
                        {
                            if ((inputCharArray[count] + offset) > 122)
                                inputCharArray[count] = Convert.ToChar(96 + (offset - (122 - inputCharArray[count])));
                            else
                                inputCharArray[count] = Convert.ToChar((inputCharArray[count] + offset));
                        }                        
                    }
                    else
                    {
                        inputCharArray[count] = Convert.ToChar((inputCharArray[count] - offset));
                    }
                }

                initialValue++;
            }

            string decryptedString = inputCharArray.ToString(); //decrypted string
            //Console.WriteLine(decryptedString);                        
            return decryptedString;
        }

        public string stringtenograph(string input) 
        {
            string[] splitString = input.Split('.');
            StringBuilder charArray = new StringBuilder(splitString[0]);
            string remainingString = input.Substring(charArray.Length, input.Length - charArray.Length);
            string output = string.Empty;

            for (int i = 0; i < charArray.Length; i++)
            {                
                if (charArray[i] >= 65 && charArray[i] <= 90) 
                    output = output + ConfigurationManager.AppSettings[charArray[i].ToString() + " "];
                else if (charArray[i] >= 97 && charArray[i] <= 122)
                    output = output + ConfigurationManager.AppSettings[charArray[i].ToString()];                    
                else if(charArray[i] == 32)
                    output = output + "SPACE";
                else
                    output = output + charArray[i];

                if (i != charArray.Length - 1)
                    output = output + " ";
            }

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings[output] == null)
            {
                configuration.AppSettings.Settings.Add(output, remainingString);
            }
            else
            {
                configuration.AppSettings.Settings.Remove(output);
                configuration.AppSettings.Settings.Add(output, remainingString);
            }

            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");

            return output;
        }

        public string decodestringtenograph(string input)
        {
            string[] splitString = input.Split('.');
            string remainingString = input.Substring(splitString[0].Length, input.Length - splitString[0].Length);
            string[] splitWords = splitString[0].Split(' ');

            if (ConfigurationManager.AppSettings[input] != null)
            {
                remainingString = ConfigurationManager.AppSettings[input.ToString()];
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings.Remove(input);
            }
            else
            {
                remainingString = "";
            }
            

            string toDecrypt = string.Empty;
            for (int i = 0; i < splitWords.Length; i++)
            {
                if (ConfigurationManager.AppSettings[splitWords[i].ToString()] != null)
                    toDecrypt = toDecrypt + ConfigurationManager.AppSettings[splitWords[i].ToString()];
                else if (splitWords[i].ToString() == "SPACE")
                    toDecrypt = toDecrypt + " ";
                else
                    toDecrypt = toDecrypt + splitWords[i];
            }

            toDecrypt = toDecrypt + remainingString;
            return toDecrypt;
        }

        public string decodstenograph(string input, string wordcount)
        {
            //string inp = "this  is test of  spaces  pl  pl  pl plthis  is test of  spaces  pl  pl  pl plthis  is test of  spaces  pl  pl  pl plthis  is test of  spaces  pl  pl  pl pl";
            string inp = input;
            int numberofwords = Convert.ToInt32(wordcount);
            StringBuilder arr = new StringBuilder(inp);
            string binarycount = string.Empty;
            string decodestring = string.Empty;
            for(int i=0;i<arr.Length - 1;i++){
                if (Convert.ToInt16(arr[i]) == 32){
                    if (Convert.ToInt16(arr[i + 1]) == 32){
                        binarycount = binarycount + "0";
                        i++;
                    }                        
                    else
                        binarycount = binarycount + "1";
                }
                if(binarycount.Length == 8){
                    if(decodestring.Length < ((numberofwords - 1)/8))
                        decodestring = decodestring + Convert.ToChar(Convert.ToInt16(binarycount, 2));
                    binarycount = string.Empty;
                }
            }
            //Console.WriteLine(decodestring);
            string remainingString = string.Empty;

            if (ConfigurationManager.AppSettings[decodestring] != null)
            {
                remainingString = ConfigurationManager.AppSettings[decodestring.ToString()];
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings.Remove(decodestring);
            }
            else
            {
                remainingString = "";
            }

            string toDecrypt = string.Empty;
            toDecrypt = decodestring;
            toDecrypt = toDecrypt + remainingString;

            return toDecrypt;
        }
    }
}