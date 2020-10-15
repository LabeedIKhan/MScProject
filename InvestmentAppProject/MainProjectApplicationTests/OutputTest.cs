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
    public class OutputTest
    {
        /*
         * 
         */
        
        [TestMethod()]
        public void AddStockOutputTest()
        {

            Output op = new Output();

            string expected = "Company Symbol: AAPLCompany Name: Apple Inc.Average: 0.0199  Maximum: 0.1962  Minimum: -0.741  Range: 0.0123  Variance: 0.0123  Stand.Dev: 0.07000";

            op.StockOutputToResultTab(expected);
            string actual = op.OutPutText;

            Assert.AreEqual(expected, actual);
            // Passed
        }

        [TestMethod()]
        public void AddWeightsAndERTest()
        {
            /*
             * This test test the method in out class that fomrats data
             * to string output to be shown in the text box at the last 
             * page and in activity log file. 
            */

            Output output = new Output();

            //Test data
            string[] symbols = { "AAPL", "MSFT" };
            double[] weight = { 75, 25 };
            double er = 100.528;
            double risk = 25.25;

            output.WeightsERToResultText(symbols, weight, er, risk);

            string expected = "Adjusted Weights\nAAPL:  w0  75 | MSFT:  w1  25 |  Expected ER : 100.528  Adjusted Risk : 2525";

            Assert.AreEqual(expected, output.OutPutText);
            // Passed
        }
        [TestMethod()]
        public void SaveToMyPortfolioTest()
        {
            Output output = new Output();

            //Test Data 
            string name = "Portfolio34"; string[] symbols = { "AAPL", "MSFT" }; double[] weights = { 40, 50 };
            double er = 52.25; double risk = 4.25;

            output.SaveToMyPortfolioList(name, symbols, weights, er, risk);

            string name1 = "Portfolio36"; string[] symbols1 = { "FB", "GOOG" }; double[] weights1 = { 75, 25 };
            double er1 = 59.25; double risk1 = 4.25;

            output.SaveToMyPortfolioList(name1, symbols1, weights1, er1, risk1);
            // Should Update the portfolio display that show saved 
            // portfolio on front page

            string[][] expected = new string[2][];
            expected[0] = new string[] { "Portfolio34", "AAPL : 40 MSFT : 50 ", "ER : 52.25", "Risk : 4.25" };
            expected[1] = new string[] { "Portfolio36", "FB : 75 GOOG : 25 ", "ER : 59.25", "Risk : 4.25" };

            //CollectionAssert.AreEqual(expected, output.GetPortfolioMainOutput);


            string[][] actual = output.GetPortfolioMainOutput;
            for (int i = 0; i < expected.Length; i++)
            {
                for (int j = 0; j < expected[i].Length; j++)
                {
                    Assert.AreEqual(expected[i][j], actual[i][j]);
                }
            }
            //Passed
            // This test tests another method as well which is used 
            // to update the main variable
        }

    }
}