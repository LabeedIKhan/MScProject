using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Threading;

namespace MainProjectApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    /* =========Charts===========
     * This class uses and external chart creation package
     * by livecharts added to project as nuget package 
     * from visual studio nuget package manager. 
     * The website of live charts is https://lvcharts.net/
     * usage idea has been taken from 
     * https://lvcharts.net/App/examples/wpf/start
     * and more information is avaiable from nuget
     * official site https://www.nuget.org/packages/LiveCharts.Wpf/
     * auther and owner BetoRodriguez 
     * To see installed packages right click on project name
     * and select Manage Nuget package and select installed packages.
     */

    public partial class MainWindow : Window
    {
        /*  The class main purpose is event handling 
         *  and GUI management, the program read no data from
         *  Grids, only row index on grid is taken for 
         *  user actions. 
         */

        DisplayManager dm;
        Output output;
       
        public MainWindow()
        {
            InitializeComponent();
            //dm = new DisplayManager();
            //CreateInitialLayout();
            //FillFrontTable();
            StartAll();  
        }

        public void StartAll()
        {
            dm = new DisplayManager();
            output = new Output();
            // Create some initila GUI componnets 
            //like pie charts and grid lables
            CreateInitialLayout();
            // Request symbols data for complay list
            // table either from local file or api
            // from DisplayManager class 
            FillFrontTable();
        }

        public void FillFrontTable()
        {
            dm.RequestSymbolList();
            // remove previous data before updating
            SymbolGrid.Items.Clear();
            string[][] list = dm.SymbolsList;
            // Binds symbol data to SymbolGrid
            SymbolGrid.ItemsSource = list;
        }
        
        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            // Eventhandler for New Button 
            // start a totally new Main GUI window
            // while keeping previous open
            new MainWindow().Show();  
        }

        private void RestartApp_Click(object sender, RoutedEventArgs e)
        {
            // Restart button event hander program shut down and
            // restart itself, application specific.
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();  
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close window from which close button is pressed
            this.Close();
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            // Exit from application not from that window
            // where it was pressed
            // Event handler for Exit Button
            System.Windows.Application.Current.Shutdown();
        }

        private void DeleteLocalFile_Click(object sender, RoutedEventArgs e)
        {
            // Event handler for Delete local 
            // delete and confirm all data 
            // has been deleted
            dm.DeleteAllLocalRecord();
        }
        private void AppUpadateAndReset_Click(object sender, RoutedEventArgs e)
        {
            // Event handler for update and reset 
            // button delete all local record and data
            // in application start application from 
            // every thing new to remove any error cause
            // by bad data. or update record from new data 
            // from apis
            dm.DeleteAllLocalRecord();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void SaveCSV_Click(object sender, RoutedEventArgs e)
        {
            /* Event handler for SaveCSV button 
             * check if user has saved a portfolio in 
             * application Selected Portfolio table
             * if there is a portfolio open file chooser window
             */
            if (output.DoesSavedPortfolioExist())
            { 
                Microsoft.Win32.SaveFileDialog savefile = new Microsoft.Win32.SaveFileDialog();
                
                savefile.DefaultExt = ".csv";

                savefile.Filter = "(.csv)|*.csv";
                // if user cancel saving 
                Nullable<bool> operation = savefile.ShowDialog();
                // if it operation hasn't been cancelled 
                if (operation == true)
                {
                   output.SaveToCSVMyPort(savefile.FileName);
                }
            }
            else
            {
           
                
            }
        }

        private void SymbolGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /* List page, company list, mouse double click 
             * event handler
             */
             // index of row double clicked
            int index = SymbolGrid.SelectedIndex;
            // index must be within array length
            if (index <= dm.SymbolsList.Length && dm.SymbolsList.Length > 0)
            {
                string value = "";
                try
                {
                    // value held in original data 
                    // that has same indexes as grid
                    value = dm.SymbolsList[index][0];
                    // if not already present
                    if (!dm.MainDataHas(value))
                    {
                        dm.GetProfileRow(value);
                    }
                    // update profile table with data
                    FillProfileTable();
                }
                catch (IndexOutOfRangeException) { }
            }
        }

        public void FillProfileTable()
        {
            /* The method updates profile table 
             * on stock tab user can multiple 
             * actions to change data presented
             * in this table, so every can update 
             * it by calling this method.
             */
            string[][] prof = dm.ProfileDisplay;
            PGrid.ItemsSource = null;
            PGrid.Items.Refresh();
            // DataGrid data source 
            PGrid.ItemsSource = prof;
            // update customize combo with new sector list
            UpdateCombo();
        }


        public void UpdateCombo()
        {
            // Once profile table is updated
            // update customize combo
            FilterCombo.ItemsSource = dm.SectorDisplay;
        }

        public void CreateInitialLayout()
        {

            /*  This method provide intial structure
             *  for numerical data grid and weights pie charts
             *  
             */
            for (int i = 0; i < 100; i++)
            {
                SymbolGrid.Items.Add(SymbolGrid.Items);
            }
            DataGridTextColumn dgtcDate = new DataGridTextColumn();
            dgtcDate.Header = "Date";
            HistoryNum.Columns.Add(dgtcDate);

            for (int i = 0; i < 60; i++)
            {
                DataGridTextColumn dgtc = new DataGridTextColumn();
                dgtc.Header = "Month " + i;
                HistoryNum.Columns.Add(dgtc);
            }
            // testbox under portfolio tab.portfolio grid 
            PortfolioResults.Text = "Portfolio Analytics";
            // update customize combo
            UpdateCombo();
            // create pie with 7 pieces
            for (int i = 0; i < 7; i++)
            {
                /* uses LiveCharts nuget package
                 * to create charts, information regarding 
                 * LiveCharts and links are given in description
                 * above
                 */
               EqualWeigth.Series.Add(new PieSeries { Title = "N/A", Values = new ChartValues<double> { 25 } });
               AdjustedWeigth.Series.Add(new PieSeries { Title = "N/A", Values = new ChartValues<double> { 25 } });
           }
           if (dm.Aplha)
           {
               ApiTextBox.Text = "Aplha Vantage";
           }
           else
           {
               ApiTextBox.Text = "Financial Modeling";
           }
       }

       private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
       {
           /* Event handler for sort combo
            * on stock tab 
            */
                string value = "";
            if (dm.ProfileDisplay.Length > 1)
            {
                value = ((ComboBoxItem)SortCombo.SelectedItem).Content as string;
            }
            // value already set in xaml
            // converted into string
            // uses Format.SortByPreferences class to sort 
            // but uses DisplayManager.SortByValue as 
            // data is stored in DisplayManager. 
            // No data value is read from grid itself
            if (value == "Price")
            {
                dm.SortbyValue(4);
            }
            else if (value == "Market Cap")
            {
                dm.SortbyValue(5);
            }
            else if (value == "PE/Ratio")
            {
                dm.SortbyValue(6);
            }
            else
            {
            }
            // Once sorting complete fill grid table with 
            // sorted data
            FillProfileTable();
        }

        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /* Remove row from Profile Grid
             * with selected sector,  
             */
            string value = "";

            var combo = sender as ComboBox;
            value = combo.SelectedItem as string;

            if (value == "Restore Original")
            {
                // if user want to restore 
                // restore from data held 
                // held in display manager
                dm.UpdateProfileDisplay();
            }
            else
            {
                // remove row with this value
                // original removal is done in Format
                // class method FilterByPreferences
                dm.FilterbyValue(value, 3);
            }
            // again update profile grid table
            FillProfileTable();
        }

        private void FilterCombo_DropDownClosed(object sender, EventArgs e)
        {
            // keeps filter combo selection clear
            // if not, will not be able to restore 
            // straightaway if row is remove
            // from remove option in drop down,
            // unless other row is removed from combo itself
            FilterCombo.SelectedItem = null;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            /* Event handler Open Web page 
             * Open companies url in browser
             * when user presses Open Wen Pages option
             * from drop down of profile grid table 
             */
            int index = PGrid.SelectedIndex;
            // check index is index of an actual array in array
            if (index >= 0 && index < dm.ProfileDisplay.Length)
            {
                int len = dm.ProfileDisplay[index].Length;
                // select second last url value from
                // array, value in array can vary
                // second last index is value of url.
                string url = dm.ProfileDisplay[index][len - 2];
                // if replaced with N/A, if was not available
                if (url != "N/A")
                {
                    // open url in broser
                    System.Diagnostics.Process.Start(url);
                }
                else
                {
                    MessageBox.Show("No Url Present for This Comapny");
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            /* Event handler for Show Description 
             * menu item, show the data held in 
             * array of profile to text to be shown
             * on another window as company description
             * converted in text in DisplayManager 
             * maethod GetDescriptionText
             */
            string description = "";

            int index = PGrid.SelectedIndex;
            if (index >= 0 && index < dm.ProfileDisplay.Length)
            {
                description = dm.GetDescriptionText(index);
                Window1 w1 = new Window1();
                // open window
                w1.Show();
                // provide text to w1 window
                w1.SetDescriptText(description);
            }
        }
 
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            // Add To Analysis Event Handler
            // Ask display manager to request 
            // stock numerical data from either api
            // 
            int index = PGrid.SelectedIndex;
            if (index >= 0 && index < dm.ProfileDisplay.Length)
            {
                // Get Identifies symbol held in DisplayManager
                dm.GetPrices(dm.ProfileDisplay[index][0]);
                // Check ===========================
                // dm.UpdateStockNumericDisplay();
                Task.Delay(1000);
                FillStockNumerictable();
                // create Main Chart in Stock tab
                //Task.Delay(100);
                CreateStockCharts();     
            }
        }

        

        private void CreateStockCharts()
        {  
            /* This method uses LiveCharts nuget package
             * to create charts, information regarding 
             * LiveCharts and links are given in description
             * above, data provided to charts
             * is date labels, actual numerical values,
             * and symbols of companies, data used
             * to get value is the actual data being shown 
             * in numerical value table
             */

            string[] symbols = new string[dm.MainNumDisplay.Length - 1];

            for (int i = 0; i < symbols.Length; i++)
            {
                symbols[i] = dm.MainNumDisplay[i + 1][0];
            }

            double[][] values = dm.GetValuesForStockCharts(symbols);

            string[] dates = dm.DateSeries;
            // clear charts to present new values
            SeriesStock.Series.Clear();
            // loop for every company
            for (int i = 0; i < symbols.Length; i++)
            {
                double[] val = values[i];
                LineSeries line = new LineSeries { Title = symbols[i], Values = new ChartValues<double>(val) };
                // create new line series add it into chart 
                SeriesStock.Series.Add(line);
            }

            List<string> dateList = new List<string>();

            for (int i = 0; i < dates.Length; i++)
            {
                dateList.Add(dates[i]);
            }
            // dates on x axises
            SeriesStock.AxisX.First().Labels = dateList;  
        }

        public void FillStockNumerictable() 
        {
            
            string[][] numerics = dm.MainNumDisplay;  
          
            HistoryNum.Columns.Clear();
            
            DataTable table = new DataTable();
            // create data cloumns dynamically 
            // for month, depending on months of longest
            // array of stock
            for (int i = 0; i < numerics[0].Length - 1 ; i++)
            {
                DataColumn col = new DataColumn(numerics[0][i], typeof(string));

                table.Columns.Add(col);
            }
            // add rows to table with added column above
            for (int i = 1; i < numerics.Length; i++)
            {
                DataRow row = table.NewRow();

                for (int j = 0; j < numerics[i].Length; j++)
                {
                    // add a value for row and column
                    row[j] = numerics[i][j];
                }
                table.Rows.Add(row);
            }
            // make datatable source of data for numerical table grid
            HistoryNum.ItemsSource = table.DefaultView;
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            /*  Event handler for remove menu item
             *  for Profile Grid uses same functionality
             *  as cutomize combo box, removes selected
             *  row from profile grid table, can be restored 
             */
            int index = PGrid.SelectedIndex;
            if (index >= 0 && index < dm.ProfileDisplay.Length)
            {
                // index is index of row, value dm.ProfileDisplay[index][0]
                // is idetifier symbol of company at 0 horizontal index
                dm.FilterbyValue(dm.ProfileDisplay[index][0], 0);
                // update profile grid 
                FillProfileTable();
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            /* Event handler for Menu Item delete
             * for profile grid table, completely remove
             * data from program variable for selected 
             * comapny. cannot be restored 
             */
            int index = PGrid.SelectedIndex;
            string value = "";
            if (dm.ProfileDisplay.Length > 0)
            {
                if (index >= 0 && index < dm.ProfileDisplay.Length)
                {
                    value = dm.ProfileDisplay[index][0];

                    dm.RemoveFromMainData(value);

                    FillProfileTable();
                }
            }
        }

        private void APICombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // Event handler for API chage combo box
            string value = ((ComboBoxItem)APICombo.SelectedItem).Content as string;

            MessageBox.Show("Two apis format is not compatible stocks numerics table will be reset \n" +
                "It is recommeded that you remove all stocks from portfolio table as well \n " +
                "and add them again");
                if (value == "Aplha Vantage")
                {
                   // tell display manager what api is 
                   // being used by set value alpha bool variable 
                   // true for alpha vantage and false from fin modeling
                    dm.Aplha = true;
                }
                else if (value == "Financial Modeling")
                {
                    dm.Aplha = false;
                }
                // Text box display for what api is currently being used
                if (dm.Aplha)
                {
                    ApiTextBox.Text = "Aplha Vantage";
                }
                else
                {
                    ApiTextBox.Text = "Financial Modeling";
                }
                // let display manager adjust for different date format
                dm.Change = true;
                // clear all numerical stock values and their dates 
                // from mainStockNumeric and individualDates dictionaries
                dm.ResetDictionary();
            // update numercal values table 
            // here empty table to clear existing data
                Task.Delay(100);
                FillStockNumerictable();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            /* Event handler for numerical Grid table 
             * menu delete to delete stock numerical 
             * values and it'dates held in 
             * mainStockNumerica and individualDates 
             * Dictinaries. 
             */

            int index = HistoryNum.SelectedIndex;

            string value = "";
            if (index >= 0 && index < dm.MainNumDisplay.Length)
            {
                value = dm.MainNumDisplay[index + 1][0];

                dm.RemoveFromDictionary(value);
                // update numercal table after deletion
                // of selected stock
                Task.Delay(100);
                FillStockNumerictable();
                // recreate stock chart with existing stocks
                CreateStockCharts();
            }
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            /* Event Handler for Statistcs
             * option in Numerical Vales Grif Table
             * Menu, Ask DisplayManager class to
             * create stock object using GenerateStock
             * method and Stock class TheStock is getter
             * of Stock object created by DisplayManager
             * These values will be display in separate window
             * that pop up when Statistcs is pressed from menu
             */

            int index = HistoryNum.SelectedIndex;
            string value = "";
            if (index >= 0 && index < dm.MainNumDisplay.Length)
            {
                value = dm.MainNumDisplay[index + 1][0];

                dm.GenerateStock(value);
                // Statistcs window 
                Window2 w2 = new Window2();

                string[][] data = new string[8][];
                // The values for stock 
                data[0] = new string[] { "Company Symbols", dm.TheStock.TheStockSymbol };
                data[1] = new string[] { "Company Name", dm.TheStock.TheStockName };
                data[2] = new string[] { "Company Average", dm.TheStock.Average.ToString() };
                data[3] = new string[] { "Company Maximum", dm.TheStock.Maximum.ToString() };
                data[4] = new string[] { "Company Minimum", dm.TheStock.Minimum.ToString() };
                data[5] = new string[] { "Company Range", dm.TheStock.Range.ToString() };
                data[6] = new string[] { "Company Variance", dm.TheStock.Variance.ToString() };
                data[7] = new string[] { "Company Standard Deviation", dm.TheStock.StanderdDeviation.ToString() };
                // pass values for charts dates and perfromance value
                // to ShowStatisticsGrid method in Statistcs.xaml.cs
                w2.ShowStatisticsGrid(data, dm.ForIndividualRORDatesCharts(value), dm.ForIndividualRORCharts(value) );
                w2.Show();
                // Start background worker to format 
                // stock object as string and show in 
                // result tab
                RunStockWorker();
            }
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            // Event handler for Add to Portfolio
            // menu option in Numerical Values Grid
            // in Stock Tab
            int index = HistoryNum.SelectedIndex;

            string value = "";

            if (index >= 0 && index < dm.MainNumDisplay.Length)
            {
                value = dm.MainNumDisplay[index + 1][0];
                // specied operation is adding stock to portfolio
                dm.PortfolioOperation(value, "ADD");

                FilePortfolioDisplayTable();

                DisplayPortfolioComponents();
                // stock background worker thread to 
                // format and display portfolio calcautions in
                // reult tab
                RunPortfolioWorker();
            }

        }

        public void FilePortfolioDisplayTable()
        {
            string[][] port = dm.PortDisplay;
            try
            { 
                PortfolioGrid.ItemsSource = null;
                PortfolioGrid.Items.Refresh();
                PortfolioGrid.ItemsSource = port;
            }
            catch (Exception) { }
        }

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            // Event handler for Remove menu option in 
            // Portfolio Grid in Portfolio tab
            // remove a selected stock from portfolio
            int index = PortfolioGrid.SelectedIndex;

            if (index >= 0 && index < dm.PortDisplay.Length)
            {
                string value = dm.PortDisplay[index][0];
                // Operation is remove 
                dm.PortfolioOperation(value, "Re");

                FilePortfolioDisplayTable();

                DisplayPortfolioComponents();
                // Again Run Portfolio backGround worker thread
                RunPortfolioWorker();
            }

        }

        public void DisplayPortfolioComponents()
        {
            /* This method uses LiveCharts nuget package
             * to create charts, information regarding 
             * LiveCharts and links are given in description
             * above
             */
             // if there is a stock in portfolio
            if (dm.PortDisplay.Length > 1)
            {
                double[] portTimeSeries = dm.ThePortfolio.PortfolioTimeSeries;
                string[] dates = dm.ThePortfolio.GetTheStockDatesLongest();
                double[] RoRs = dm.ThePortfolio.ThePortFolioRoRs;

                SeriesPortfolio.Series.Clear();

                if (portTimeSeries.Length > 0)
                {   
                    // chart values from portfolio timeseries 
                    LineSeries portfolioSeries = new LineSeries { Title = "Portfolio", Values = new ChartValues<double>(portTimeSeries) };
                    // Add to main line chart in Portfolio tab
                    SeriesPortfolio.Series.Add(portfolioSeries);
                }
 
                List<string> dateList = new List<string>();
                if (dates != null)
                {
                    for (int i = 0; i < dates.Length; i++)
                    {
                        dateList.Add(dates[i]);
                    }
                    // date for portfolio timeseries chart
                    SeriesPortfolio.AxisX.First().Labels = dateList;
                }

                string[] symbol = dm.ThePortfolio.GetTheStockTickers;
                double[] Weight = dm.ThePortfolio.PortfolioWeights;

                AdjustedWeigth.Series.Clear();

                for (int i = 0; i < symbol.Length; i++)
                {
                    // weight cannot be less than 0 
                    if (Weight[i] < 0)
                    {
                        Weight[i] = 0;
                    }
                    Weight[i] = Math.Round(Weight[i], 4);
                    // Add stock identifier symbol and weight to adjusted
                    // weight pie chart
                    AdjustedWeigth.Series.Add(new PieSeries { Title = symbol[i], Values = new ChartValues<double> { Weight[i] } });
                }

                double Allweights = 0.0;
                for (int i = 0; i < Weight.Length; i++)
                {
                    Allweights += Weight[i];
                }
                // equal weight 
                Allweights = Math.Round(Allweights / Weight.Length, 2);

                EqualWeigth.Series.Clear();
                for (int i = 0; i < symbol.Length; i++)
                {
                    // Add stock identifier symbol and weight to 
                    // equal weight pie chart
                    EqualWeigth.Series.Add(new PieSeries { Title = symbol[i], Values = new ChartValues<double> { Allweights } });
                }
                // show individual Rate Of Return of Stock 
                // in portfolio as column chart in Portfolio tab
                RORBarChart.Series.Clear();
                if (RoRs != null)
                {
                    for (int i = 0; i < symbol.Length; i++)
                    {
                        // loop for stocks in portfolio add identifier and ror value
                        RORBarChart.Series.Add(new ColumnSeries { Title = symbol[i], Values = new ChartValues<double> { RoRs[i] } });
                    }

                }
                List<double> Invar = dm.ThePortfolio.GetPortInVar;
                VariancePie.Series.Clear();
                // Individual variance of stock in portfolio
                // shown as column charts in Advance tab
                if (Invar != null)
                {
                    for (int i = 0; i < symbol.Length; i++)
                    {
                        // loop for stocks in portfolio add identifier and variance
                        VariancePie.Series.Add(new ColumnSeries { Title = symbol[i], Values = new ChartValues<double> { Invar[i] } });
                    }
                }
                List<double> InSD = dm.ThePortfolio.GetPortInSDs;
                SDPie.Series.Clear();
                // Individual SD of stock in portfolio
                // shown as column charts in Advance tab
                if (InSD != null)
                {
                    for (int i = 0; i < symbol.Length; i++)
                    {
                        // loop for stocks in portfolio add identifier and SD
                        SDPie.Series.Add(new ColumnSeries { Title = symbol[i], Values = new ChartValues<double> { InSD[i] } });
                    }
                }
                // text box in Portfolio Tab
                EqualWeightBox.Text = "ER: " + dm.ThePortfolio.PortfolioER.ToString() + " Risk: " + (dm.ThePortfolio.PortfolioSD * 100) + "%";
                AdjustedWeightBox.Text = "Adjusted: ER: " + dm.ThePortfolio.AdjustedReturn.ToString() + " Risk: " + (dm.ThePortfolio.AdjustedSD * 100) + "%";
                PortfolioResults.Text = "Adjusted: ER: " + dm.ThePortfolio.AdjustedReturn.ToString() +
                                        " Variation " + Math.Round(dm.ThePortfolio.AdjustedVariance, 8).ToString() +
                                        " Risk: " + (dm.ThePortfolio.AdjustedSD * 100) + "%";
                // Create variance covariacne and 
                // correaltion grid tables in Advance tab
                CreateMatrixInAdvanceTab();
            }
            else
            {
                // Clear Following Components
                EqualWeightBox.Text = "";

                AdjustedWeightBox.Text = "";
                // Show message in main portfolio text box
                PortfolioResults.Text = "Add Atleast two Assets to Get calculations";

                CreateMatrixInAdvanceTab();

                SDPie.Series.Clear();

                VariancePie.Series.Clear();

                RORBarChart.Series.Clear();

                SeriesPortfolio.Series.Clear();

                EqualWeigth.Series.Clear();

                AdjustedWeigth.Series.Clear();
                
                EqualWeigth.Series.Add(new PieSeries { Title = "N/A", Values = new ChartValues<double> { 0.0 } });

                AdjustedWeigth.Series.Add(new PieSeries { Title = "N/A", Values = new ChartValues<double> { 0.0 } }); 
            }
        }

        public void CreateMatrixInAdvanceTab()
        {
            string[,] CovTable = dm.ThePortfolio.Covtable;
            string[,] CorrTable = dm.ThePortfolio.Corrtable;
            // Create VCOV table 
            DataTable vcovtable = CreateMatrixTables(CovTable);
            VCOVGrid.Columns.Clear();
            // VCOV Grid data source
            VCOVGrid.ItemsSource = vcovtable.DefaultView;

            CorrGrid.Columns.Clear();
            // Create Correlation table
            DataTable corrtable = CreateMatrixTables(CorrTable);
            // Correllation Grid data source
            CorrGrid.ItemsSource = corrtable.DefaultView;
        }

        public DataTable CreateMatrixTables(string[,] mydata)
        {
            DataTable table = new DataTable();

            for (int i = 0; i < mydata.GetLength(0); i++)
            {
                // generate columns
                DataColumn col = new DataColumn("Company" + i, typeof(string));
                table.Columns.Add(col);
            }
            for (int i = 0; i < mydata.GetLength(0); i++)
            {
                // generate rows
                DataRow row = table.NewRow();

                for (int j = 0; j < mydata.GetLength(1); j++)
                {
                    row[j] = mydata[i, j];
                }
                table.Rows.Add(row);
            }
            return table;
        }

        
        public void RunStockWorker()
        {
            // The method is called by MenuItem_Click_6
            // when user generates as stock object
            BackgroundWorker stockworker = new BackgroundWorker();
            // method to run when called 
            stockworker.DoWork += StockWorkerDoWork;
            // when task is complete
            stockworker.RunWorkerCompleted += WorkersCompleted;
            stockworker.RunWorkerAsync();
        }

        private void StockWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // stock object to formated as string
            output.StockOutputToResultTab(dm.TheStock.ToString());
        }

        public void RunPortfolioWorker()
        {
            /* This method is called by MenuItem_Click_7
             * when a stock is added to portfolio and 
             * MenuItem_Click_8 when a stock is removed from 
             * portfolio
             */
            BackgroundWorker portworker = new BackgroundWorker();
            // add method to be start background work 
            portworker.DoWork += PortWorkerDoWork;
            // called on completion 
            portworker.RunWorkerCompleted += WorkersCompleted;
            portworker.RunWorkerAsync();  
        }

        private void PortWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            // provide output class correlation table and
            // covariacne table to be converted as string text
            // and portfolio description also
            // to be shown in result tab
            output.CreateCovCorrResultTable("Corr", dm.ThePortfolio.Corrtable);
            output.CreateCovCorrResultTable("Cov", dm.ThePortfolio.Covtable);
            output.WeightsERToResultText(dm.ThePortfolio.GetTheStockTickers, dm.ThePortfolio.PortfolioWeights,
                                    dm.ThePortfolio.AdjustedReturn, dm.ThePortfolio.AdjustedSD);
        }

        private void WorkersCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Called when background task is complete
            // update result tab text box 
            MainTextOutPut.AppendText(output.OutPutText+ "\n" + "" +
                "______________________________________________________________" +
                "______________________________________________________________"+ "\n");
            // clear text variable in output class
            output.OutPutText = "";
        }

        private void SavePortfolio_Click(object sender, RoutedEventArgs e)
        {
            if (dm.PortDisplay != null && dm.PortDisplay.Length > 1)
            {
               // Save selected portfolio values in to output class
               // to be shown as selected portfolio or saved as csv
                output.SaveToMyPortfolioList(dm.ThePortfolio.GetPortfolioName, dm.ThePortfolio.GetTheStockTickers,
                                         dm.ThePortfolio.PortfolioWeights, dm.ThePortfolio.AdjustedReturn,
                                         dm.ThePortfolio.AdjustedSD);
                // Portfolio Selected Display on List tab
                UpdateFrontPortfolioDisplay();
            }
        }

        public void UpdateFrontPortfolioDisplay()
        {
            string[][] portfolioOuput = output.GetPortfolioMainOutput;
            FrontPortfolioOutput.ItemsSource = null;
            FrontPortfolioOutput.Items.Refresh();
            FrontPortfolioOutput.ItemsSource = portfolioOuput;
        }
    }
}
