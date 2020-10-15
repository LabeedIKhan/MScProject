using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Windows;
using System.IO;
using System.Runtime.CompilerServices;

namespace MainProjectApplication
{
    public class Output
    {
        /* The purpose of output class is to format calaculation 
         * and their labels in string so they can displayed in 
         * result tab text output, fromt portfolio list table after
         * user saves it, in activity file, and to be stored 
         * in csv file. 
         */

        private string MainOutPutText = "";
        private static string MainLog = "";
        private List<string[]> MainPortfolioOuput = new List<string[]>();
        private List<string[]> myportFolio = new List<string[]>();
        private string[][] PortolioOutPutDisplay;

        //Getter And Setters ============
        public string OutPutText { get => MainOutPutText; set => MainOutPutText = value; }
        public string GetMainLog { get => MainLog; }
        public List<string[]> GetPortFolioOutputList { get => MainPortfolioOuput; }
        public string[][] GetPortfolioMainOutput { get => PortolioOutPutDisplay; }

        public Output()
        {

        }

        public void StockOutputToResultTab(string stockouput)
        {
            /* Called from MainWindow.xaml.cs method 
             * StockWorkerDoWork provide text output
             * to result tab when user presses statistics
             * for individual stock on stock tab numerical
             * values tab. No formatting need as stock
             * class implments ToString() and has been formatted 
             * there. 
             */

            MainOutPutText += stockouput;
            
            WriteLogs(stockouput);
            
        }

        public void CreateCovCorrResultTable(string table, string[,] tab)
        {
            /* This method is called from MainWindow.xaml.cs
             * method StockWorkerDoWork run on background 
             * thread format tables for result tab table while
             * user can continue with analysis using 
             * mainthread. 
             */

            /* Test For Multi Threading
             * if thread.sleep(5000) is uncommented 
             * it will show delayed output update 
             * in result tab when user has peroformed 
             * statistics analysis and portolfio analysis 
             */
            //Thread.Sleep(5000);
            string tableName = "";

            if (table == "Corr")
            {
                tableName = "Correlation table" + "\n";
            }
            else
            {
                tableName = "Covariance table" + "\n";
            }

            for (int i = 0; i < tab.GetLength(0); i++)
            {
                for (int j = 0; j < tab.GetLength(1); j++)
                {
                    tableName += String.Format("{0, 20}",tab[i, j] + "  ");
                }
                tableName += "\n";
            }

            MainOutPutText += tableName;

            WriteLogs(tableName);
            
        }

        public void WeightsERToResultText(string[]symbols, double[] weights, double er, double risk)
        {
            /*
             *   This method is called from mainwindow.xaml.cs 
             *   method PortWorkerDoWor and runs on background 
             *   thread and formats the line correlation and covariance
             *   table in result tab text. 
             */

            if (weights.Length > 0)
            {
                string mainERAdjusted = "Adjusted Weights" +"\n";

                for (int i = 0; i < symbols.Length; i++)
                {
                    weights[i] = Math.Round(weights[i], 3);

                    mainERAdjusted += symbols[i] + ":  " + "w" + i + "  " + weights[i]+" | ";
                  
                }
                mainERAdjusted += " Expected ER : " + er.ToString();
                // * 100 to be changed to % for display 
                mainERAdjusted += "  Adjusted Risk : " + (risk * 100).ToString();

                MainOutPutText += mainERAdjusted;

                // Also writes it in log file. 
                WriteLogs(mainERAdjusted);   
            }
        }

        public void SaveToMyPortfolioList(string name, string[] symbols, double[] weights, double er, double sd)
        {
            /*  This method is called from MainWindow.xaml.cs
             *  class method SavePortfolio_Click when user 
             *  want to save portfolio after selection, 
             *  the portfolio selected is shown on List tab
             *  Selected Portfolio which can then be stored
             *  as csv 
             */

            bool alreadySaved = false;

            for (int i = 0; i < myportFolio.Count; i++)
            {
                if (myportFolio[i][0] == name)
                {
                    alreadySaved = true;
                }
            }
            // if this portfolio is not already saved
            // then saves it
            if (!alreadySaved)
            {
                string[] myportfolisave = new string[4];

                myportfolisave[0] = name;

                string symWeights = "";

                for (int i = 0; i < symbols.Length; i++)
                {
                    symWeights += symbols[i] + " : " + weights[i] + " ";

                }

                myportfolisave[1] = symWeights;
                myportfolisave[2] = "ER : " + er;
                myportfolisave[3] = "Risk : " + sd;

                myportFolio.Add(myportfolisave);
                UpdatePortfolioOuputDisplay();
            }
        }

        public bool DoesSavedPortfolioExist()
        {
            /*
             * This method is called from mainwindow.xaml.cs
             * mathod SaveCSV_Click() for open filechooser window
             * to confirm if user has saved portfolio in 
             * list tab portfolio table so save file selection
             * window will be allowed to open if no
             * portfolio is saved, then pressing savecsv button 
             * will do nothing. 
             */
            if (myportFolio.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdatePortfolioOuputDisplay()
        {
            /* This method is written to be called 
             * to update SelectedPortfolio table 
             * display when all other requirements
             * have been completed. 
             */

            string[][] temporary = new string[myportFolio.Count][];

            for (int i = 0; i < temporary.Length; i++)
            {
                temporary[i] = myportFolio[i];    
            }
             PortolioOutPutDisplay = temporary;
        }

        // Syncronized as shared by multiple threads.
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void WriteLogs(string record)
        {
            MainLog += record + "\n";
            try
            {
                File.WriteAllText("PSActivityLogs.txt", MainLog);
            }
            catch (Exception) { }
        }
  
        public void SaveToCSVMyPort(string filename)
        {
            /*
             * This method let user save end result of portfolio
             * analysis to csv file the filename is provided 
             * by filechooserwindow for c# after user name the 
             * file to be saved. 
             * */

            List<string> writetofile = new List<string>();

            for (int i = 0; i < myportFolio.Count; i++)
            {
                string temporarystring = "";
                for (int j = 0; j < myportFolio[i].Length; j++)
                {
                    // Add , so can be stored as csv
                    temporarystring += myportFolio[i][j]+ ",";
                }
                writetofile.Add(temporarystring);
            }

            try
            {
                // true so if saved in the same file will append data
                using (StreamWriter file = new StreamWriter(@filename, true))
                {   
                    string write = "";
                    foreach (var x in writetofile)
                    {
                        write += x + "\n";
                    }

                    file.WriteLine(write);
                    
                }
            }
            // It will let user know that file hasn't been saved
            catch (Exception) { MessageBox.Show("Error Occured During Writing CSV"); }
        }
    }
    
}
