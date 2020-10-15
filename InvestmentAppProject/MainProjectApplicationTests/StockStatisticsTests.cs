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
    public class StockStatisticsTests
    {
        /*
         * Only ten values have been used which are taken from 
         * api AlphaVantage itself that provided data to application 
         * https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol=AAPL&apikey=1RVXQFD5ODJY0JHK
         * Dates might change as month passes
         * Ten values are sufficinet enough to show accuracy of caluclation
         * Price values are 11 as ror loength is one less than price
         * The Rate of return expected test values are calcaulted using excel
         * 
         * 
         * Please See Unit_Test_Calc_Sheet file and main report for more 
         * information
         */
        double[] pricesAPI = { 204.5435, 213.04, 197.92, 175.07, 200.67, 189.95, 173.15, 166.44, 157.74, 178.58, 218.86 };
        double[] rorExcel = {-0.039882182, 0.076394503, 0.130519221, -0.127572632,0.056435904, 0.0970257, 0.040314828,0.055154051,
                             -0.116698398, -0.184044595};
        [TestMethod()]
        public void RateOfReturnTest()
        {
            /*
            * Test rate of Return formula obtained from CFI 
            * https://corporatefinanceinstitute.com/resources/knowledge/finance/rate-of-return-guide/
            * Tests StockStatistics RateOfReturn(double[] number) method
            */
            StockStatistics sst = new StockStatistics();

            double[] testData = sst.RateOfReturn(pricesAPI);

            for (int i = 0; i < testData.Length; i++)
            {
                Assert.AreEqual(Math.Round(rorExcel[i], 4), Math.Round(testData[i], 4));
            }

            // Test Result Passed, Loop has been written as aparently 
            // Visual Studio is Unable to Test equality of Arrays themselves.

        }

        [TestMethod()]
        public void GetMaximumTest()
        {
            StockStatistics sst = new StockStatistics();

            // 0.130519221 calcuated from excel
            Assert.AreEqual(0.130519221, sst.GetMaximum(rorExcel));

            // Test Result Passed
        }

        [TestMethod()]
        public void GetMinimumTest()
        {
            StockStatistics sst = new StockStatistics();

            // -0.184044595 calcuated from excel
            Assert.AreEqual(-0.184044595, sst.GetMinimum(rorExcel));
            // Passed 
        }

        [TestMethod()]
        public void GetAverageTest()
        {
            StockStatistics sst = new StockStatistics();

            // -0.002903357 calcuated from excel
            // As average mothod rounds values 
            double expected = Math.Round(-0.00123536, 4);
            Assert.AreEqual(expected, sst.GetAverage(rorExcel));
            //Passed
        }

        [TestMethod()]
        public void GetVarianceTest()
        {
            StockStatistics sst = new StockStatistics();

            double expected = Math.Round(0.010547867, 5);

            // 0.010701833 calcuated from excel

            Assert.AreEqual(expected, sst.GetVariance(rorExcel));

            // Passed
        }

        [TestMethod()]
        public void GetSDTest()
        {
            StockStatistics sst = new StockStatistics();

            // 0.103449663 calculated using Excel
            double expected = Math.Round(0.10270281, 5);

            Assert.AreEqual(expected, sst.GetSD(rorExcel));
        }

        [TestMethod()]
        public void GetRangeTest()
        {
            StockStatistics sst = new StockStatistics();

            // Test range max - min  
            // calculated using excel
            double expected = 0.314563816;

            Assert.AreEqual(expected, sst.GetRange(rorExcel));
        }

        [TestMethod()]
        public void StockObjectTest()
        {

            // The above method test the method that generate 
            // statistical calculation for stock this method 
            // test the stock object that uses them to get calculations.

            Stock stock = new Stock("APPLE", "Apple Incorporate");
            stock.CalculateStatistics(rorExcel);

            string expected = "Company Symbol: APPLE\nCompany Name: Apple Incorporate\n" +
                              "Average: -0.0012  Maximum: 0.130519221  Minimum: -0.184044595" +
                              "  Range: 0.314563816  Variance: 0.01055  Stand.Dev: 0.1027  ";

            Assert.AreEqual(expected, stock.ToString());
            // Passed
        }
    }
}