using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace MainProjectApplication
{
    public class InternetAPI
    {
        Thread AnimateTh;

        /*  The class uses external nuget package 
         *  NewTonSoft.Json to parse json string 
         *  it is included in project through 
         *  Visual Studio nuget package manager 
         *  package auther James Newton-King and nuget link is 
         *  https://www.nuget.org/packages/Newtonsoft.Json/
         *  more information is available in project's 
         *  installed nuget packages by right clicking on project
         *  selecting Manage Nuget Packages and Installed 
         *  The how to use idea was present on 
         *  stackoverflow by AZ Oct 2012 Parse Json String
         *  in C# retireved from 
         *  https://stackoverflow.com/questions/12676746/parse-json-string-in-c-sharp
         *  Apis by Financial Modeling free
         *  https://financialmodelingprep.com/developer/docs/
         *  and Alpha Vantage free with key
         *  https://www.alphavantage.co/documentation/
         */


        internal class WebClientCustomize : WebClient
        {
            // This internal class has been created to set
            // timeout though not the best way but 
            // it will cause the webclient to through exception 
            // after 6 seconds to stop unresponsive web request blocking 
            // User Interface then user can try again or even another company
            // or api, The internal class extends WebClient 
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                // Timeout will occur after 6 seconds if no response 
                // is recieved from apis within that time 
                request.Timeout = 6000;
                return request;
            }

        }
       
        public string[][] SendRequestForSymbols()
        {
            /* This method send request to the API financial Modeling
             * to return a list of companies along with their symbols
             * AAPL for apple, MSFT for Microsoft
             * is Called From DisplayManager's method RequestSymbolList
             * This method will only be called if no data is held locally 
             * or it has been deleted,updated or program is new 
             * on computer if program is unable to use text file for storage
             */

            var json = "";
            List<string[]> Symlist = new List<string[]>();
            string[][] symbols = new string[1][];

              // internal webclientcutomize class is not used here
              // as it is not intended to timeout request after
              // six seconds as it is essntial data for application
              // But it does not required to be updated very frequently
                using (WebClient webclient = new WebClient())
                {
                    try
                    {    
                        //json = webclient.DownloadString("https://financialmodelingprep.com/api/v3/stock/list?apikey=  ");
                        json = webclient.DownloadString("http://localhost:8080/symbolist");
                    
                    }
                    catch (Exception) { return null; }
                    
                    // Parse json array
                    // information of using newtonsoft.json
                    // is available in description above
                    JArray obj = JArray.Parse(json);
                
                    foreach (JObject root in obj)
                    {
                    // These fields will be obtained from api
                    string ticker; string price; string name;
                    try
                    {
                        ticker = (string)root["Symbol"];
                        price = (string)root["Price"];
                        name = (string)root["Name"];  
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                        string[] tickersSym = { ticker, price, name };
                        Symlist.Add(tickersSym);
                    }
                }

                symbols = new string[Symlist.Count][];
                for (int i = 0; i < symbols.Length; i++)
                {
                    symbols[i] = Symlist[i].ToArray();
                }
                 // sort array by company names 
                 //symbols.OrderBy(entry => entry[2]).ToArray();
                 return symbols;
            
        }

        public async Task<string[][]> GetHistoricalDataFinModel(string symbol)
        {
            /* Get stock historic price performane 
             * from Financial Modeling called from 
             * DisplayManager method GetPrices method
             * symbol identifier of the company 
             */
            List<string[]> history = new List<string[]>();
            string[][] historydata;// = new string[1][];
            var json = "";


            using (WebClientCustomize webclient = new WebClientCustomize())
            {
                try
                {

                    StartAnimationWindowThread();
                   // json = webclient.DownloadString("https://financialmodelingprep.com/api/v3/historical-price-full/" + symbol + "?serietype=line&serieformat=array&datatype=json");
                    json = await Task.Run(() => webclient.DownloadString("https://financialmodelingprep.com/api/v3/historical-price-full/" + symbol + "?apikey=  "));
                    
                    FinishAnimationWindowThread();
                }
                catch (Exception) {

                    /*
                     * Thread.Sleep is added here to give
                     * time to animation thread to run before terminating
                     * becuase if not connected to internet 
                     * the expection will be thrown very fast and 
                     * application might go into break mode or crash
                     * identified during testing, it was resolved 
                     * by adding Thread.Sleep below
                     */

                    Thread.Sleep(500);
                    // if exception thrown by downloadstring 
                    // end animation 
                    FinishAnimationWindowThread();
                    return null; }
            }
            // information of using newtonsoft.json
            // is available in description above
            JObject jobj = JObject.Parse(json);

            JToken historical = jobj["historical"];

            foreach (var x in historical)
            {
                    string date = (string)x["date"];
                    string close = (string)x["close"];

                    string[] his = { symbol, date, close };

                    history.Add(his);
             }
                
                historydata = new string[history.Count][];
                for (int i = 0; i < historydata.Length; i++)
                {
                    historydata[i] = history[i].ToArray();
                }
                string[][] monthlyhistory = ConvertToMonthly(historydata);

                return monthlyhistory;
            
        }
        public string[][] ConvertToMonthly(string[][] data)
        {
            /* This method is called by GetHistoricalDataFinModel
             * method and it convert daily data to monthly as 
             * daily data would be to much to be handled by program 
             * and the other api used for same data reuturns
             * minthly data so this method changes daily data
             * to monthly by averaging all daily data from a month
             */

            List<string[]> monthlyAverage = new List<string[]>();

            List<string> datelist = new List<string>();

            List<int> dateindexs = new List<int>();


            // First remove days from data and keep months
            // create a new list on monthly data. 
            for (int i = 0; i < data.Length; i++)
            {
                if (!datelist.Contains(data[i][1].Substring(0, 7)))
                {
                    datelist.Add(data[i][1].Substring(0, 7));
                    dateindexs.Add(i);
                }
            }

            // change daily data from a month to one 
            // monthly data value by taking average of all
            // days in a month

            for (int i = 0; i < dateindexs.Count - 1; i++)
            {
                int counter = 0;
                double num = 0.0;
                string[] therow = new string[3];
                for (int j = dateindexs[i]; j < dateindexs[i + 1]; j++)
                {

                    num += double.Parse(data[j][2]);
                    counter++;
                }
                double ave = 1.0 * num / counter;
                ave = Math.Round(ave, 3);
                string avestr = ave.ToString();
                therow[0] = data[i][0]; therow[1] = datelist[i].ToString(); therow[2] = avestr;
                monthlyAverage.Add(therow);
            }

            string[][] monthlyPrice = new string[monthlyAverage.Count][];

            for (int i = 0; i < monthlyPrice.Length; i++)
            {
                monthlyPrice[i] = monthlyAverage[i];
            }

            // Array Reverse
            //=================================================
            // reverse the order of array so both apis provide 
            // data with same format.

            // monthlyPrice = DataReverse(monthlyPrice);

            //==================================================
                return monthlyPrice;
        }

        public async Task<string[]> GetCompanyProfile(string symbol)   
        {
            /* Retrieves company profile data from the folowing Apis
             * as required data is present in two apis both apis 
             * are called and data of interest is taken 
             */

            string profile = "";
            string matrix = "";
            string[] data = { };

            using (WebClientCustomize webclient = new WebClientCustomize())
            {
                try
                {
                    StartAnimationWindowThread();
                    
                    //profile = webclient.DownloadString("https://financialmodelingprep.com/api/v3/company/profile/" + symbol);
                    profile =  await Task.Run(() => webclient.DownloadString("https://financialmodelingprep.com/api/v3/profile/" + symbol + "?apikey=  "));

                    

                    // Makes requests a little slow so double clicking
                    // doesn't generate very frequent requests 
                    // The api iteself is very fast.
                    // Half second is not much from user perspective
                    // and it will only send request if data is not held 
                    // locally, let's animation thread start and finish 

                   
                    
                   // matrix = webclient.DownloadString("https://financialmodelingprep.com/api/v3/company-key-metrics/" + symbol);
                    matrix = await Task.Run(() =>  webclient.DownloadString("https://financialmodelingprep.com/api/v3/key-metrics/" + symbol + "?apikey=  "));

                    FinishAnimationWindowThread();
                }
                catch (Exception)
                {
                    /* Any problem during request or request
                     * timeout return null so program know 
                     * and tell user to select another company 
                     * or check for possible problem. 
                     */

                    /*
                    * Thread.Sleep is added here to give
                    * time to animation thread to run before terminating
                    * becuase if not connected to internet 
                    * the expection will be thrown very fast and 
                    * application might go into break mode or crash
                    * identified during testing, it was resolved 
                    * by adding Thread.Sleep below
                    */
                    Thread.Sleep(500);
                    FinishAnimationWindowThread();
                    return null;
                }
            }
            try
            {
                // information of using newtonsoft.json
                // is available in description above
                //JObject profileOb = JObject.Parse(profile);

                //JObject matrixOb = JObject.Parse(matrix);
                // The values that are being taken from the api


                JArray profileArr = JArray.Parse(profile);
                JToken profileRoot = profileArr[0];

                
                string price = (string)profileRoot["price"];
                string marketcap = (string)profileRoot["mktCap"];
                string name = (string)profileRoot["companyName"];
                string industry = (string)profileRoot["industry"];
                string website = (string)profileRoot["website"];
                string description = (string)profileRoot["description"];
                string sector = (string)profileRoot["sector"];

                

                JArray metaArr = JArray.Parse(matrix);
                JToken metarray = metaArr[0];

                string DateM = (string)metarray["date"];
                string MarkCap = (string)metarray["marketCap"];
                string PEratio = (string)metarray["peRatio"];

                string[] profileDataArray = { symbol, name, industry, sector, price, MarkCap, PEratio, description, website, DateM };
                        
                data = CheckForMissingProfileValues(profileDataArray);
             }
             catch (Exception)
            {  
                /* return null so program can tell user
                 * that some problem occured during request
                 * or may be some import part of data about this 
                 * company is not present. 
                 */
                return null;
            }
                return data;   
        }

        public string[] CheckForMissingProfileValues(string[] data)
        {
            /*
             * This method will do some checks on api and if some
             * values are missing they will be replaced by 
             * N/A or 0.0 so next steps work
             */
            if (data[0] == null || data[0] == "")
            {
                // If no symbol present for stock rejects 
                // data and return null so user can be asked 
                // to select another company. 
                return null;
            }

            string[] replacments = { "N/A", "N/A", "N/A", "0.0", "0.0", "0.0", "N/A", "N/A", "N/A" };

            for (int i = 1; i < data.Length; i++)
            {
                if (data[i] == null || data[i] == "")
                {
                    data[i] = replacments[i - 1];
                }
             }
            return data;
        }

        public async Task<string[][]> GetHistoricalDataAlphaVan(string symbol)     
        {

            /* Get stock historic price performane 
             * from alpha vantage called from 
             * DisplayManager method GetPrices method
             * symbol identifier of the company
             */

            List<string[]> history = new List<string[]>();
            string[][] historydata;
            string json = "";
            using (WebClientCustomize webclient = new WebClientCustomize())
            {
                try
                {
                    StartAnimationWindowThread();
                    
                    json = await Task.Run(() => webclient.DownloadString("https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol=" + symbol + "&apikey=  "));
                    
                    FinishAnimationWindowThread();
                }
                catch (Exception) {

                    /*
                    * Thread.Sleep is added here to give
                    * time to animation thread to run before terminating
                    * becuase if not connected to internet 
                    * the expection will be thrown very fast and 
                    * application might go into break mode or crash
                    * identified during testing, it was resolved 
                    * by adding Thread.Sleep below
                    */
                    //Thread.Sleep(500);
                    // if exception is thrown by downloadstring
                    FinishAnimationWindowThread();
                    return null;
                }
            }  
            try
            {
                // information of using newtonsoft.json
                // is available in description above
                JObject Ob = JObject.Parse(json);

                JToken MonSeries = Ob["Monthly Time Series"];

                JObject MonOb = (JObject)MonSeries;
                // There were some problems parsing data from
                // object so convert it into array 
                string s = "[" + MonOb.ToString() + "]";
                // parse data as array
                JArray obj = JArray.Parse(s);

                foreach (JObject x in obj)
                {
                    foreach (KeyValuePair<String, JToken> y in x)
                    {
                        string Date = y.Key;

                        string value = (string)y.Value["4. close"];

                        string[] his = { symbol, Date.Substring(0, 7), value };

                        history.Add(his);
                    }

                }
            }
            catch (Exception) { return null; }

            /* Below code reduces the data from company 
             * that have data more than 10 years
             * very high amount of data had slowed the 
             * stock tab and not all companies started same 
             * time high variation can cause calculation 
             * not be very accurate. 
             */

            try
            {
                if (history.Count > 120)
                {
                    int over = history.Count - 120;

                    historydata = new string[history.Count - over][];
                }
                else
                {
                    historydata = new string[history.Count][];
                }
                for (int i = 0; i < historydata.Length; i++)
                {
                    historydata[i] = history[i].ToArray();
                }
            }
            // return null if no data is present for 
            // spcified company. so user can be informed
            // and asked to choose another company
            catch (Exception) { return null; }

            return historydata;

        }
        public void StartAnimationWindowThread()
        {
            /* Below code will start thread to run 
             * Animation window once this method is called
             */
            AnimateTh = new Thread(new ThreadStart(StartAnimationThread));
            AnimateTh.SetApartmentState(ApartmentState.STA);
            AnimateTh.IsBackground = true;
            AnimateTh.Start();

        }
        private void StartAnimationThread()
        {
            try
            {
                /* The animation is on a separate Window 
                 * to be run on separate thread while mainthread 
                 * wait for reponse form apis. 
                 */
                Window3 win3 = new Window3();
                win3.Show();
                System.Windows.Threading.Dispatcher.Run();
            }
            catch (Exception)
            {
                // stops code from throwing AbortException again 
                Thread.ResetAbort();
                AnimateTh = null;
            }
        }
        public void FinishAnimationWindowThread()
        {
            /* This method is called from all method 
             * above that download json responses 
             * after download is complete
             * cause thread to abort by throwing 
             * abort expection 
             */
            if (AnimateTh.IsAlive)
            {
                try
                {   
                    // Terminate Animation thread
                    AnimateTh.Abort();
                    AnimateTh = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception)
                {  
                    Thread.ResetAbort();
                    AnimateTh = null;
                }
            }
        }

        public string[][] DataReverse(string[][] monthlyPrice)
        {
            string[] datereverse = new string[monthlyPrice.Length];
            string[] pricereverse = new string[monthlyPrice.Length];

            for (int i = 0; i < monthlyPrice.Length; i++)
            {
                datereverse[i] = monthlyPrice[i][1]; pricereverse[i] = monthlyPrice[i][2];
            }
            Array.Reverse(datereverse); Array.Reverse(pricereverse);

            for (int i = 0; i < monthlyPrice.Length; i++)
            {
                monthlyPrice[i][1] = datereverse[i]; monthlyPrice[i][2] = pricereverse[i];
            }

            return monthlyPrice;
        }

    }
}
