using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace MainProjectApplication
{
    class DisplayManager 
    {
        
        public DisplayManager()
        {
            
        }
        private StockStatistics stockStats = new StockStatistics();
        private PortfolioMaths portfolioStats = new PortfolioMaths();
        private Stock stock;
        private Portfolio myportfolio;

        private string[][] symbolslist = new string[][] { };

        private List<string[]> profileList = new List<string[]>();

        private string[][] profileDisplay = new string[][] { };

        private string[] Dates = new string[] { };

        private Dictionary<string, double[]> mainStockNumeric = new Dictionary<string, double[]>();

        private string[][] mainNumericDisplay = new string[][] { };

        private Dictionary<string, string[]> individualDates = new Dictionary<string, string[]>();

        private List<string> symbolOrder = new List<string>();

        private int portfoliocount = 0;

        private Dictionary<string, string[]> MainPortListDisplay = new Dictionary<string, string[]>();

        private string[][] MainPortArrayDisplay = new string[][] { };

        private Dictionary<string, string[]> PortIndDates = new Dictionary<string, string[]>();

        private Dictionary<string, double[]> AllAssetinPort = new Dictionary<string, double[]>();

        private string[] sectorDisplay = { "Not Set" };

        private bool aplha = true;

        private bool change = false;

        

        //================Getter==and==Setters==================//

        public string[][] SymbolsList { get => symbolslist; }
        public string[][] ProfileDisplay { get => profileDisplay; }
        public string[] DateSeries { get => Dates; }
        public string[] SectorDisplay { get => sectorDisplay; }
        public string[][] MainNumDisplay { get => mainNumericDisplay; }
        public Stock TheStock { get => stock; }
        public Portfolio ThePortfolio { get => myportfolio; }
        public bool Aplha { get => aplha; set => aplha = value; }
        public string[][] PortDisplay { get => MainPortArrayDisplay; }
        public bool Change { get => change; set => change = value; }


        public void RequestSymbolList()
        {
            /*  This method is called everytime program 
             *  starts to provide CompanyList on List tab
             *  called from MainWindow.xaml.cs method
             *  FillFrontTable(). 
             */

            InternetAPI jsonData = new InternetAPI();

            DataFileManager dfm = new DataFileManager();
            // check local record first if present
            string[][] localSymbol = dfm.GetLocalSymbolRecord();
            // if no local send request
            if (localSymbol == null)
            {
                localSymbol = jsonData.SendRequestForSymbols();
                // if record imported  
                if (localSymbol != null)
                {
                    // record on local text fill 
                    // for future use 
                    dfm.RecordLocalSymbolsList(localSymbol);
                    // update display arrays
                    symbolslist = localSymbol;
                }
                else
                {
                    // show error message
                    PrintInternetExceptionMessageBox("S");
                }
            }
            else
            {
                // if present locally
                symbolslist = localSymbol;
            }
        }
        public string GetTickerSymbol(int index)
        {
            // return identifier of a company
            // from company list this 
            // will be use to retrieve Profile 
            // from file or api
            return symbolslist[index][0];
        }

        public async void GetProfileRow(string symbol)          
        {
            /* The purpose of this method is to 
             * get comapny profile data row either from
             * file by using DataFileManager class or
             * From API, If data is not present locally 
             * it will also update local data once data
             * has been called from InternetAPI. 
             * If data cannot be obtained in some cases
             * shows user message box. 
             */

            DataFileManager dfilemanager = new DataFileManager();
            InternetAPI pjson = new InternetAPI();

            // if not present locally 
            string[] companyrow = dfilemanager.AccessLocalProfileRecord(symbol);
            if (companyrow == null)
            {
                // get company profile from api
                companyrow = await pjson.GetCompanyProfile(symbol);      

                if (companyrow != null)
                {
                    // if data has been imported update
                    // main record and display
                    dfilemanager.UpDateLocalProfileRecord(companyrow);
                    profileList.Add(companyrow);
                    UpdateProfileDisplay();
                }
                else
                {
                    // show user message box
                    PrintInternetExceptionMessageBox("C");
                }
            }
            else
            {
                // if data held locally 
                // and and other conditions are fullfilled
                // defined in DataFileManager add it 
                // to main record update profile display
                profileList.Add(companyrow);
                UpdateProfileDisplay();
            }
        }
        public void UpdateProfileDisplay()
        {
            // move data from main record to display
            string[][] displayList = new string[profileList.Count][];

            for (int i = 0; i < displayList.Length; i++)
            {
                displayList[i] = profileList[i];
            }
            profileDisplay = displayList;
            // produce sector list for customize combo box
            ProduceSectorList();
        }


        // =====Format Data Using Format Class====
        // for two combo boxes. 
        // Sort in Descending order
        public void SortbyValue(int col)
        {
            Format format = new Format();
            profileDisplay = format.SortByPreferences(profileDisplay, col);
        }
        // Remove particular sector company from profile table
        public void FilterbyValue(string value, int col)
        {
            Format format = new Format();
            profileDisplay = format.FilterByPreferences(profileDisplay, value, col);
            // once a particular sector removed
            // again produce sector list for 
            // customize combo box. 
            ProduceSectorList();
        }

      //  private bool change = false;
      //  public bool Change { get => change; set => change = value; }

        public async void GetPrices(string symbol)          /////////   async
        {
            InternetAPI pjson = new InternetAPI();

           
            if (!mainStockNumeric.ContainsKey(symbol))
            {
                string[][] historydetails;

                /* 
                 * Depending on the selection of user
                 * which api is currently being used 
                 * if it is alphavantage then alpha
                 * would be true else otherwise
                 */

                if (aplha)
                {
                    historydetails = await pjson.GetHistoricalDataAlphaVan(symbol);  
                }
                else
                {
                    historydetails = await pjson.GetHistoricalDataFinModel(symbol);  
                }

                if (historydetails != null)
                {
                    string[] dates = new string[historydetails.Length];
                    double[] price = new double[historydetails.Length];

                    for (int i = 0; i < dates.Length; i++)
                    {
                        dates[i] = historydetails[i][1];
                        price[i] = double.Parse(historydetails[i][2]);
                    }
                    // takes longest date from imported stocks data
                    if (Dates.Length < dates.Length)
                    {
                        Dates = dates;
                    }
                    // if api has been change and 
                    else if (change)
                    {
                        Dates = dates; change = false;
                    }
                    individualDates.Add(symbol, dates);

                    mainStockNumeric.Add(symbol, stockStats.RateOfReturn(price));

                    UpdateStockNumericDisplay();
                }
                else
                {
                    PrintInternetExceptionMessageBox("N");   
                }
            }
            else
            {
            }
        }

        public void UpdateStockNumericDisplay()
        {
            /* Called from MainWindow.xaml.cs method
             * MenuItem_Click_2 and GetPrices, ResetDictionary 
             * and RemoveFromDictionary of this class 
             * updates main stock numerical display table
             * defines columns, and add labels
             * Data from the Array of Arrays with be
             * used to generate column and labels 
             * for main grid in stock tab
             */

            string[][] displayArr = new string[mainStockNumeric.Count + 1][];

            displayArr[0] = new string[Dates.Length + 1];
            // Add dates label for first value then followed by months
            displayArr[0][0] = "Dates";
            // add dates on first horizontal array 
            for (int i = 0; i < Dates.Length; i++) // missing 1
            {
                displayArr[0][i + 1] = Dates[i];
            }
            // convert mainStockNumeric dictionary to Array
            // of Array also add labels of company name
            // before numerical values
            int counter = 1;

            foreach (var x in mainStockNumeric)
            {
                displayArr[counter] = new string[x.Value.Length + 1];
                // add labels
                displayArr[counter][0] = x.Key;
                // add numerical values
                for (int i = 0; i < x.Value.Length; i++)
                {
                    displayArr[counter][i + 1] = x.Value[i].ToString(); // missing 1
                }
                counter++;
            }

            mainNumericDisplay = displayArr;
        }


        public void ProduceSectorList()
        {
            /* This method is called everytime user takes
             * any action that add or remove company profile
             * rows from profile table it produces
             * sector list to be shown in customize combo box
             * called from UpdateProfileDisplay and 
             * FilterByValue method of this class
             */
            List<string> sector = new List<string>();

            for (int i = 0; i < profileDisplay.Length; i++)
            {
                // one sector value is selected from 
                // multiple stock if they have same sector
                // to be displayed in customize combo 
                if (!sector.Contains(profileDisplay[i][3]))
                {
                    sector.Add(profileDisplay[i][3]);
                }
            }
            // add restore original value also
            sector.Add("Restore Original");
            string[] sectorArr = new string[sector.Count];

            for (int i = 0; i < sectorArr.Length; i++)
            {
                sectorArr[i] = sector[i];
            }
            sectorDisplay = sectorArr;
        }

        public void RemoveFromMainData(string value)
        {
            /* Called from MainWindow.xaml.cs method
             * MenuItem_Click_4 delete 
             * from profile table right click menu
             * delete particular row from main record
             */
            for (int i = 0; i < profileList.Count; i++)
            {
                string value2 = profileList[i][0];

                if (value == value2)
                {
                    profileList.RemoveAt(i);
                }
            }
            //again update profile display after removal
            UpdateProfileDisplay();
        }

        public string GetDescriptionText(int index)
        {
            /* On Pressing show description button from 
             * profile table below code transfer data from
             * this class to Description.xaml.cs class to 
             * be display in Description Window
             */
            string[] decriptionLabels = {"Symbol", "Name", "Industry", "Sector",
                                         "Price", "Market Cap", "PE/Ratio",
                                          "Description", "Website", "Date"};
            int len = profileDisplay[index].Length;

            string desc = "";

            for (int i = 0; i < len; i++)
            {
                desc += decriptionLabels[i] + ":  " + profileDisplay[index][i] + "\n";
            }
            return desc;
        }
       
        public bool MainDataHas(string value)
        {
            /* called by SymbolGrid_MouseDoubleClick of
             * MainWindow.xaml.cs to confirm if profileTable 
             * already has required profile row if not then
             * send request for profile to file or api
             */
            bool has = false;

            for (int i = 0; i < profileList.Count; i++)
            {
                if (profileList[i][0] == value)
                {
                    has = true;
                    break;
                }
            }
            return has;
        }

        public static void PrintInternetExceptionMessageBox(string command)
        {
            if (command == "C")
            {
                MessageBox.Show("Unable to Retireve Required data \n Some data might be missing or is unavaialbe" +
                    "\n or your computer is not connected to Internet \n Please check your internet connection \n" +
                    "or try another company");
            }
            else if (command == "S")
            {
                MessageBox.Show("Uable to retireve company list please check your internet connection \n" +
                    "As no local record is present");
            }
            else if (command == "N")
            {
                MessageBox.Show("Uable to retireve historical Data \n data might be missing or your computer is not connected \n" +
                    "check your internet connection or select another company \n" +
                    "Or Try Changing Api");
            }
            else
            {
                MessageBox.Show("Unkown exception occurred");
            }

        }

        public void ResetDictionary()
        {
            // when api is changed called from
            // APICombo_SelectionChanged
            // from MainWindow.xaml.cs
            mainStockNumeric.Clear();
            individualDates.Clear();
            UpdateStockNumericDisplay();
        }

        public void RemoveFromDictionary(string symbol)
        {
            /* This method is called from 
             * MainWindow.xaml.cs method MenuItem_Click_5
             * when a stock is deleted from menu
             */
            mainStockNumeric.Remove(symbol);
            individualDates.Remove(symbol);
            UpdateStockNumericDisplay();
        }

        public void GenerateStock(string symbol)
        {
           /*  Create Stock object depending on given symbol of company
            *  Passes Rate of Return values for that stock 
            *  Stockmis ceated when user presses Statisitcs
            *  menu option for Numerical Values grid table
            */
            string name = "";
           
            for (int i = 0; i < symbolslist.Length; i++)
            {
                /*  It is possible that user can delete 
                 *  a stock from Profile but still want
                 *  details to be shown for stock which
                 *  is present in Numerical Grid table 
                 *  So get name stored in Symbol List
                 *  the main company list
                 */
                if (symbolslist[i][0] == symbol)
                {
                    name = symbolslist[i][2];
                    break;
                }
            }
            // pass symbol and name to stock constructor 
            stock = new Stock(symbol, name);
            // get value numarical values Ror
            // dictionary using stock identifier symbol
            double[] val = mainStockNumeric[symbol];
            // pass rate of return value for 
            // calcualtions
            stock.CalculateStatistics(val);
        }

        public double[][] GetValuesForStockCharts(string[] symbols)
        {
            /*  Called from MainWindow.xaml.cs method
             *  CreateStockCharts() returns numerical 
             *  value for main stock chart in stock tab
             */
            double[][] values = new double[symbols.Length][];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = mainStockNumeric[symbols[i]];
            } 
            return values; 
        }
         
        public void PortfolioOperation(string symbol, string type)
        {
            /*  Called from MenuItem_Click_7 and MenuItem_Click_8
             *  when use add stock to portfoio and remove stock
             *  from portfolio grid, it modify actual record
             *  held about portfolio and create a new portfolio
             *  object everytime called.Type can be add or other
             *  if add a new stock will be added else removed 
             *  the selected stock from portfolio
             */

            if (type == "ADD")
            {
                // portfolio grid three columns
                string[] display = new string[3];
                // first symbol of company
                display[0] = symbol;

                for (int i = 0; i < symbolslist.Length; i++)
                {
                    if (symbolslist[i][0] == symbol)
                    {
                        // second column name of company
                        display[1] = symbolslist[i][2];
                        break;
                    }
                }
                // third column average of rate of return for stock
                double stockAver = Queryable.Average(mainStockNumeric[symbol].AsQueryable());
                stockAver = Math.Round(stockAver, 4);
                display[2] = stockAver.ToString();
                // check same is not repeated
                if (!symbolOrder.Contains(symbol))
                {
                    // add stock idetifier to list
                    symbolOrder.Add(symbol);
                    // add symbol and it values into 
                    // dictionaries 
                    PortIndDates.Add(symbol, individualDates[symbol]);
                    AllAssetinPort.Add(symbol, mainStockNumeric[symbol]);
                    // create portfolio
                    CreatePortfolio();
                }
                // Dictionary for Portfolio Grid display
                if (!MainPortListDisplay.ContainsKey(symbol))
                {
                    MainPortListDisplay.Add(symbol, display);
                }
            }
            else
            {
                // if operation is to remove a stock 
                // check if there is a stock in portfolio record
                if (symbolOrder.Contains(symbol))
                {
                    symbolOrder.Remove(symbol);
                    PortIndDates.Remove(symbol);
                    AllAssetinPort.Remove(symbol);
                    // Again create portfolio
                    CreatePortfolio();
                }
                // update dictionary for portfolio display
                if (MainPortListDisplay.ContainsKey(symbol))
                {
                    MainPortListDisplay.Remove(symbol);
                }
            }
            UpdateMainPortDisplay();
        }

        public void UpdateMainPortDisplay()
        {
            /*  Update Portfolio Dsiplay table 
             *  by moving portfolio stocks to 
             *  mainPortArrayDispaly to be shown
             *  as Grid in portfolio tab
             */

            string[][] tempDisPlay = new string[symbolOrder.Count][];

            for (int i = 0; i < tempDisPlay.Length; i++)
            {
                tempDisPlay[i] = MainPortListDisplay[symbolOrder[i]];
            }
            MainPortArrayDisplay = tempDisPlay;
        }

        public void CreatePortfolio()
        {
            List<double[]> assetList = new List<double[]>();
            string[] symbol = new string[symbolOrder.Count];
            List<string[]> dateList = new List<string[]>();

            /*  Get rate of return and individual dates for stock 
             *  from Dictionaries modified by portfolio operation
             */
            for (int i = 0; i < symbol.Length; i++)
            {
                symbol[i] = symbolOrder[i];
                assetList.Add(AllAssetinPort[symbol[i]]);
                dateList.Add(PortIndDates[symbol[i]]);
            }
            myportfolio = new Portfolio();
            // pass idetifier symbols, rate of returns, dates of stocks
            // in portfolio, and count of portfolio 
            myportfolio.GeneratePortfolio(symbol, assetList, dateList, portfoliocount);
            portfoliocount++;
        }

        public void DeleteAllLocalRecord()
        {

            /* When user wants to delete local data or update/reset
             * confirm actions that needs to be taken show result of action
             * called from DeleteLocalFile_Click and AppUpadateAndReset_Click
             * from MainWindow.xaml.cs for Delete and Update/Reset Button
             */
            MessageBoxResult mbr = System.Windows.MessageBox.Show("Do you want to delete all local record", "Confirm Deletion",
                System.Windows.MessageBoxButton.YesNo);

            if (mbr == MessageBoxResult.Yes)
            {
                DataFileManager dfm = new DataFileManager();
                // delete all files and if sucessfully deleted confirm
                bool confirm = dfm.DeleteAllLocalRecord();

                if (confirm == true)
                {
                    MessageBox.Show("Local Record Sucsessfully Deleted");
                }
                else
                {
                    MessageBox.Show("Unable to delete local record");
                }
            }
            else
            {

            }
        }


        /*
         * The pupose of these methods is to link information 
         * in this class about stock earning and date to 
         * pop up window open when user presses statisitcs
         * Statistics.xaml but it is controlled by 
         * MianWindow.xaml.cs these are just method called to
         * supply stored data in this class
         */
        public double[] ForIndividualRORCharts(string symbol)
        {
            return mainStockNumeric[symbol];
        }

        public string[] ForIndividualRORDatesCharts(string symbol)
        {
            return individualDates[symbol];
        }

    }
}
        
        
    
