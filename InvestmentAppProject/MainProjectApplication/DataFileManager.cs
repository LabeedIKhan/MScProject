using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace MainProjectApplication
{

    /*
     * The Purpose of this class is to provide substitute functinality 
     * of the data base, When user double click list of company table 
     * program first checks this AccessLocalRecord( method , then selects
     * a row/tuple in file by key names, and check whether it is outdated. 
     * If not present or outdated or have been deleted or any exception 
     * method return null, and the according to conditions written in 
     * calling method in DisplayManager class full file or particular 
     * row/tuple is upadted. This class also handle storage of list of comapnies
     * at front page but they are called altogether and updated alltogether 
     * using GetLocalRecord() and RecordLocalSymbolsList()
     * unlike Profile of companies which uses alltogether as well as tuple
     * depending on condition. 
     */
    public class DataFileManager
    {
        Dictionary<string, string> dictable = new Dictionary<string, string>();

        private static readonly string profilefilename = "Profile.txt";

        private static readonly string symbolfilename = "SymbolsList.txt";



        public string[] AccessLocalProfileRecord(string symbol)
        {
            try
            {
                // File exist perform function else return null
                // so program can call data from api and then automatically 
                // create new file and store data in it. 
                if (File.Exists(profilefilename))
                {
                    // if file has data store and is not emplty.
                    string[] file = File.ReadAllLines(profilefilename);
                    if (file.Length > 0)
                    {
                        // if frist row has data
                        if (file[0] != "")
                        {
                            // the loop over it 
                            for (int i = 0; i < file.Length; i++)
                            {
                                // pass row to ReadOnlyValid 
                                // so data read has , to prevent 
                                // below method throw indexoutofrangexception
                                // any exception thrown will cause data to be 
                                // called from api and file to overwritten
                                if (ReadOnlyValid(file[i]))
                                {
                                    //  try
                                    //  {

                                    string[] tokens = file[i].Split('|');
                                    string Rkey = tokens[0];
                                    string RValue = tokens[0];// + ",";
                                    for (int j = 1; j < tokens.Length; j++)
                                    {
                                        RValue += "|" + tokens[j];
                                    }
                                    // key would be symbol and value would
                                    // be data tuple, the value would also
                                    // itself have key idetifier. 
                                    dictable.Add(Rkey, RValue);
                                    // }
                                    // catch (Exception) {  return null; }
                                }
                            }
                            string[] data = { "" };
                            if (dictable.ContainsKey(symbol))
                            {
                                // check if required tuple/tow idetified 
                                // by symbol is present locally, if it has 
                                // get it from dictionary else return null
                                // so that row will be called from api
                                string value = dictable[symbol];

                                string[] tokens = value.Split('|');
                                // check if date added at end is not previous month
                                // or date is not present
                                if (tokens[tokens.Length - 1] == GetDate())
                                {

                                    string[] temp = new string[tokens.Length - 1];

                                    for (int i = 0; i < temp.Length; i++)
                                    {
                                        temp[i] = tokens[i];
                                    }
                                    data = temp;

                                    return data;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception) { return null; }
        }

        public void UpDateLocalProfileRecord(string[] data)
        {
            // Get Symbol from data 
            string key = data[0];
            string value = "";

            for (int i = 0; i < data.Length; i++)
            {
                // how program would identify start of new value
                // add | as spliting char
                value += data[i] + "|";
            }
            // put date at end
            value += GetDate();
            // check if dictionary has key 
            // if it has remove it re add it 
            // with row from api
            if (dictable.ContainsKey(key))
            {
                dictable.Remove(key);
                dictable.Add(key, value);
            }
            else
            {
                // else just add new row from api
                dictable.Add(key, value);
            }

            /* Covert all data in dictionary to array 
             * to be written in file
             */
            int counter = 0;
            string[] writedata = new string[dictable.Count];
            foreach (var x in dictable)
            {
                writedata[counter] = x.Value;
                counter++;
            }
            try
            {
                File.WriteAllLines(profilefilename, writedata);
            }
            catch (IOException)
            {
                
            }
        }


        private string GetDate()
        {
            /* Get Date and Time from 
             * system and get month and year
             * to be added to end of profile row/tuple
             * or to check what data is not outdated
             */

            DateTime date = DateTime.Today;
            string datestr = date.ToString();
            string[] datearr = datestr.Split('/');
            string datenow = datearr[1] + datearr[2];
            string[] datemy = datenow.Split(' ');
            // return date for example like this 082019
            return datemy[0];
        }

        private bool ReadOnlyValid(string row)
        {

            /* This method perform as additonal 
             * checks to confirm data it is reading from 
             * file is the data it stored with |
             * it is called from AccessLocalProfileRecord()
             * in same class
             */

            bool hascoma = false;
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] == '|')
                {
                    hascoma = true;
                    break;
                }
            }
            if (hascoma)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RecordLocalSymbolsList(string[][] symbols)
        {
            /* called from DisplayManager RequestSymbolList
             * method record the data recieved from api in text 
             * file, Data it records in Company List on List tab
             */

            string[] symNames = new string[symbols.Length];

            try
            {
               
                  for (int i = 0; i < symNames.Length; i++)
                  {
                     
                      try
                      {
                          string line = "";
                          for (int j = 0; j < symbols[i].Length; j++)
                          {
                            // Add , to distinguish two different values
                            // of string

                              if (symbols[i][j] == null || symbols[i][j] == "")
                              {
                                symbols[i][j] = "N/A";
                              }

                               symbols[i][j] = symbols[i][j].Replace('\n', ' ');

                               line += symbols[i][j] + "|";


                           }
                        symNames[i] = line;
                      }
                      catch(Exception)
                      {
                        
                      }
                   }
               

                //File.WriteAllLines(symbolfilename, symNames);

                using (StreamWriter file = new StreamWriter(symbolfilename, false))
                {
                    foreach (var x in symNames)
                    {
                        file.WriteLine(x);
                    }
                }
            }
            catch (Exception) { }

        }

        public string[][] GetLocalSymbolRecord()
        {
            /* 
             * Called from displaymanager method 
             * RequestSymbolList perform checks on data
             * and return data if all condition are statisfied 
             * any exception will cause return null 
             * and program will call new data from api
             * and rewrite old data that might have caused that error
             * exception can be throw due to io reason
             * data format reasons. Any exception here
             * is least likely to affect any performance. 
             */

            try
            {
                string[] record = { "" };
                if (File.Exists(symbolfilename))
                {
                   
                    record = File.ReadAllLines(symbolfilename);
                    if (record.Length > 0)
                    {
                        
                        if (record[0] != "")
                        {
                            
                            string[][] data = new string[record.Length][];
                            
                            for (int i = 0; i < record.Length; i++)
                            {
                                string[] vals;
                                 // try
                                 // {
                                
                                vals = record[i].Split('|');
                                
                                    data[i] = new string[] { vals[0], vals[1], vals[2] };
                                
                                //   }
                                // catch (Exception) { MessageBox.Show(i+" breaks here " ); }
                                 
                            }
                         
                            return data;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch(Exception)
            {
               
                return null;
            }
        }

       public bool DeleteAllLocalRecord()
        {

            /* Called from displaymanager class which
             * remove all data held locally but keeps
             * files, return status of files 
             */
            try
            {
                File.WriteAllText(symbolfilename, "");
                File.WriteAllText(profilefilename, "");
                string symbol = File.ReadAllText(symbolfilename);
                string profile = File.ReadAllText(profilefilename);

                if (symbol == "" && profile == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }
        }

       
    }
}
