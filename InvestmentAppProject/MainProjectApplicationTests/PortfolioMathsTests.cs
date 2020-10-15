using Microsoft.VisualStudio.TestTools.UnitTesting;
using MainProjectApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProjectApplication.Tests
{
    [TestClass()]
    public class PortfolioMathsTests
    {
        /*
         * These data set have been calculated using excel 
         * from the price data obtain from apis in browser 
         * ALPHA VANTAGE aplhavantage.co
         * https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol=MSFT&apikey=1RVXQFD5ODJY0JHK
         * https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol=AAPL&apikey=1RVXQFD5ODJY0JHK
         * https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol=SIRI&apikey=1RVXQFD5ODJY0JHK
         * source of RoR formula is CFI retirieved from
         * https://corporatefinanceinstitute.com/resources/knowledge/finance/rate-of-return-guide/
         * formula (Current Value - Initial(previous)Value)/ Initial(previous) value
         * and add 100 if require in percentage or can be used wihtout it
         * They have been converted from prices to earnings 
         * 
         * Please See Unit_Test_Calc_Sheet file and main report for more 
         * information 
         */

        double[] RORofAAPL = {-0.03988218, 0.076394503, 0.130519221, -0.127572632,0.056435904, 0.0970257, 0.040314828,0.055154051,
                             -0.116698398, -0.184044595};

        double[] RORofMSFT = {-0.00804579, 0.017243953, 0.083117723, -0.052986217, 0.107342717, 0.052753727, 0.072776022, 0.028157921,
                              -0.084047254, 0.038198671};

        double[] RORsofSIRI = {-0.04073482, 0.121863799, 0.050847458, -0.08605852, 0.024691358, -0.043844857, 0.017152659, 0.021015762,
                                -0.083467095, 0.034883721};

        [TestMethod()]
        public void GetCovarianceTest()
        {
            // Value calculated using excel inbuilt covariance formula
            double expected = 0.003800204;

            PortfolioMaths pmaths = new PortfolioMaths();

            // This test require two variables
            double actual = pmaths.GetCovariance(RORofAAPL, RORofMSFT);

            Assert.AreEqual(Math.Round(expected, 5), actual);

            //Passed
        }

        [TestMethod()]
        public void GetCorrelationTest()
        {
            // double expected calculated using excel
            double expected = 0.648874568;

            PortfolioMaths pmath = new PortfolioMaths();

            double actual = pmath.GetCorrelation(RORofAAPL, RORofMSFT);

            Assert.AreEqual(Math.Round(expected, 5), actual);

            //Passed 
        }

        [TestMethod()]
        public void GetPortfolioTimeSeriesTest()
        {
            // Calculated using excel
            double[] expected = {-0.02396399, 0.046819228, 0.106818472, -0.090279425, 0.08188931, 0.074889713, 0.056545425,
            0.041655986, -0.100372826, -0.072922962 };

            PortfolioMaths pm = new PortfolioMaths();

            double[][] testinput = new double[2][];
            testinput[0] = RORofAAPL;
            testinput[1] = RORofMSFT;

            double[] actual = pm.GetPortfolioTimeSeries(testinput);

            for (int i = 0; i < expected.Length; i++)
            {
                // As real values are being rounded in algorythm
                Assert.AreEqual(Math.Round(expected[i], 4), actual[i]);
            }
            // Passed
        }

        [TestMethod()]
        public void ValidFormatTest()
        {
            // This test does not require any special data 
            // as this methods function is only to make arrays
            // with two different lenght same by changing missing 
            // values at end with average of that particular array
            // as the length of all arrays should be length of the 
            // longest array for program to calculate matrics and as
            // explined in report about why to deal with missing values

            double[][] TestFormat = new double[3][];
            TestFormat[0] = new double[] { 52.25, 6.2, 85.25, 74.25, 85.240, 0.36, 85.0, 85.36 };
            TestFormat[1] = new double[] { 85.25, 74.01, 0.369, 41.23, 85.1 };
            TestFormat[2] = new double[] { 85.36, 41.25, 74.36 };

            PortfolioMaths pm = new PortfolioMaths();

            // This will return data after making all arrays equal
            // replacing missing later vlaues with averages
            double[][] actualdata = pm.ValidFormat(TestFormat);

            int actual = 0;
            for (int i = 0; i < actualdata.Length; i++)
            {
                for (int j = 0; j < actualdata[i].Length; j++)
                {
                    actual++;
                }
            }

            int expected = TestFormat[0].Length * 3;

            Assert.AreEqual(expected, actual);

            //Passed
            // Average of TestFormat[2] will be 66.99 calculated using calculater 

            Assert.AreEqual(66.99, actualdata[2][TestFormat[0].Length - 1]);

            // This will also confirm correct value at correct location 
            // of third array using length of longest array.
            // passed
        }
        // Test 5
        [TestMethod()]
        public void PortfolioErTest() // Unadjusted (no optimization weights) 
        {
            PortfolioMaths pm = new PortfolioMaths();

            double[][] testinput = new double[3][];
            testinput[0] = RORofAAPL;
            testinput[1] = RORofMSFT;
            testinput[2] = RORsofSIRI;

            // if no weights given algorythm will automotically 
            // apply equal weights null represent no weight

            double actual = pm.PortfolioEr(testinput, null);

            double expected = 0.008616911;// from excel 

            Assert.AreEqual(Math.Round(expected, 4), actual);

            // Passed 
        }
        // Test 6
        [TestMethod()]
        public void PortfolioAdjustedErTest() // Adjusted with Test weights
        {

            PortfolioMaths pm = new PortfolioMaths();

            double[][] testinput = new double[3][];
            testinput[0] = RORofAAPL;
            testinput[1] = RORofMSFT;
            testinput[2] = RORsofSIRI;

            double[] weightsTest = { 0.21, 0.52, 0.27 };

            double actual = pm.PortfolioEr(testinput, weightsTest);

            double expected = 0.013416606;// from excel 

            Assert.AreEqual(Math.Round(expected, 4), actual);

            // Passed 
        }

        [TestMethod()]
        public void GetWeightsTest()
        {
            PortfolioMaths pm = new PortfolioMaths();

            double[] expected = { 0, 98.45425600, 6.32456371 };

            double[][] input = new double[3][];
            input[0] = RORofAAPL;
            input[1] = RORofMSFT;
            input[2] = RORsofSIRI;

            double[] actual = pm.GetWeights(input);
            for (int i = 0; i < input.Length; i++)
            {
                // Round to remove little decimal anomaly
                Assert.AreEqual(Math.Round(expected[i], 4), Math.Round(actual[i], 4));
            }
        }
        [TestMethod()]
        public void PortfolioLongestDateTest()
        {
            /*
             * The return method in portfolio is supposed to return
             * dates assciated with the stock with the longest length
             * for labelling purposes below is the test expected is the 
             * longest date. 
             */
            List<string[]> dates = new List<string[]>();
            List<double[]> testStocks = new List<double[]>();

            string[] longest = { "08/2019", "08/2019", "08/2019", "08/2019", "08/2019", "08/2019", "08/2019", "08/2019" };
            string[] mid = { "08/2019", "08/2019", "08/2019", "08/2019", "08/2019" };
            string[] shortest = { "08/2019", "08/2019", "08/2019", "08/2019" };

            string[] symbols = { "Sym1", "Sym2", "Sym3" };

            // must be rotated 
            dates.Add(shortest); dates.Add(longest); dates.Add(mid);

            Portfolio p = new Portfolio();

            p.GeneratePortfolio(symbols, testStocks, dates, 0);

            Assert.AreEqual(longest.Length, p.GetTheStockDatesLongest().Length);

        }

        [TestMethod()]
        public void PortfolioStockIndividualVarianceTest()
        {
            List<double[]> testStocks = new List<double[]>();
            List<string[]> dates = new List<string[]>();

            string[] symbols = { "AAPL", "MSFT", "SIRI" };

            testStocks.Add(RORofAAPL);
            testStocks.Add(RORofMSFT);
            testStocks.Add(RORsofSIRI);

            Portfolio p = new Portfolio();

            p.GeneratePortfolio(symbols, testStocks, dates, 0);

            double[] expected = { 0.010547867, 0.003251829, 0.003792645 };

            List<double> actual = p.GetPortInVar;

            for (int i = 0; i < expected.Length; i++)
            {
                // It is rounded two time in main calculations
                expected[i] = Math.Round(expected[i], 5);
                Assert.AreEqual(Math.Round(expected[i], 4), actual[i]);
            }


        }
        [TestMethod()]
        public void PortfolioStockIndividualSDTest()
        {
            List<double[]> testStocks = new List<double[]>();
            List<string[]> dates = new List<string[]>();
            string[] symbols = { "AAPL", "MSFT", "SIRI" };

            testStocks.Add(RORofAAPL);
            testStocks.Add(RORofMSFT);
            testStocks.Add(RORsofSIRI);

            Portfolio p = new Portfolio();

            p.GeneratePortfolio(symbols, testStocks, dates, 0);

            double[] expected = { 0.10270281, 0.057024808, 0.061584458 };

            List<double> actual = p.GetPortInSDs;

            for (int i = 0; i < expected.Length; i++)
            {
                // It is rounded two time in main calculations
                expected[i] = Math.Round(expected[i], 5);
                Assert.AreEqual(Math.Round(expected[i], 4), actual[i]);
            }
        }

        [TestMethod()]
        public void PortfolioNameTest()
        {
            List<double[]> testStocks = new List<double[]>();
            List<string[]> dates = new List<string[]>();
            string[] symbols = { "AAPL", "MSFT", "SIRI" };
            Portfolio p = new Portfolio();
            p.GeneratePortfolio(symbols, testStocks, dates, 7458);
            Assert.AreEqual("Portfolio 7458", p.GetPortfolioName);
        }
        [TestMethod()]
        public void PortfolioStockSymbolTest()
        {
            List<double[]> testStocks = new List<double[]>();
            List<string[]> dates = new List<string[]>();
            string[] symbols = { "AAPLXYZ", "MSFTXY", "SIRIZAV" };

            Portfolio p = new Portfolio();
            p.GeneratePortfolio(symbols, testStocks, dates, 0);

            string[] actual = p.GetTheStockTickers;

            for (int i = 0; i < symbols.Length; i++)
            {
                Assert.AreEqual(symbols[i], actual[i]);
            }
        }

        
    }
}