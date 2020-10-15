using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProjectApplication
{
    public class Portfolio
    {

         private string PortfolioName;
         private double portfolioER;
         private double[,] correlationMatrix = new double[,] { };
         private double[,] covarianceMatrix = new double[,] { };
         private double portfolioVariance;
         private double portfolioSD;
         private double[] portfolioWeights;
         private double adjustedReturn;
         private double adjustedVariance;
         private double adjustedSD;
         private double[] portfolioTimeSeries;

        private string[,] corrtable;
        private string[,] covtable;

        private List<double> InVariances = new List<double>();
        private List<double> InSDs = new List<double>();

        public double PortfolioER { get => portfolioER; }
        public double[,] CorrelationMatrix { get => correlationMatrix; }
        public double[,] CovarianceMatrix { get => covarianceMatrix; }
        public double PortfolioVariance { get => portfolioVariance; }
        public double PortfolioSD { get => portfolioSD; }
        public double[] PortfolioWeights { get => portfolioWeights; }
        public double AdjustedReturn { get => adjustedReturn; }
        public double AdjustedVariance { get => adjustedVariance; }
        public double AdjustedSD { get => adjustedSD; }
        public double[] PortfolioTimeSeries { get => portfolioTimeSeries; }

        public string[,] Corrtable { get => corrtable; set => corrtable = value; }
        public string[,] Covtable { get => covtable; set => covtable = value; }

        private string[] StockSymbol;
        public string[] GetTheStockTickers { get => StockSymbol; }

        private List<string[]> StockDates;


        private double[] RoRsOfStocksInport;

        public double[] ThePortFolioRoRs { get => RoRsOfStocksInport; }

        public List<double> GetPortInVar { get => InVariances; }
        public List<double> GetPortInSDs { get => InSDs; }

        public string GetPortfolioName { get => PortfolioName; set => PortfolioName = value; }

        public void GeneratePortfolio(string[] symbol, List<double[]> list,  List<string[]> dates, int portfolionum)
         {
            /* Main Setter math generates all calculations
             * for stock using PortfolioMath.cs class
             * the calculations require correct sequence 
             */

            PortfolioName = "Portfolio " + portfolionum;
            // stock identifiers
            StockSymbol = symbol;
            // historic dates of all stocks in portfolio
            StockDates = dates;
                double[][] matrix = new double[list.Count][];
                // change list to matrix
                for (int i = 0; i < list.Count; i++)
                {
                    matrix[i] = list[i].ToArray();
                }
                PortfolioMaths portfolioCal = new PortfolioMaths();
                // symetric length of all arrays
                double[][] format = portfolioCal.ValidFormat(matrix);

                covarianceMatrix = portfolioCal.CovMatrix(format, list.Count);

                correlationMatrix = portfolioCal.CorrMatrix(format, list.Count);

                portfolioER = portfolioCal.PortfolioEr(format, null);

                portfolioVariance = portfolioCal.PortfolioVariance(format, null, covarianceMatrix);

                portfolioSD = portfolioCal.PortfolioSD(portfolioVariance);

                portfolioWeights = portfolioCal.GetWeights(format);// covmatirx

                adjustedVariance = portfolioCal.PortfolioVariance(format, portfolioWeights, covarianceMatrix);

                adjustedReturn = portfolioCal.PortfolioEr(format, portfolioWeights);

                adjustedSD = portfolioCal.PortfolioSD(adjustedVariance);

            if (list.Count > 0)
            {
                portfolioTimeSeries = portfolioCal.GetPortfolioTimeSeries(format);   
            }
            else
            {
                portfolioTimeSeries = new double[] { };
            }
                corrtable = portfolioCal.MyMatrixTable(symbol, correlationMatrix, "cor");
                covtable = portfolioCal.MyMatrixTable(symbol, covarianceMatrix, "cov");

            CreateRoRsForPort(list);

            CalculateVarAndSDs(list);
            //ProducedIndividualVarSd(list);
        }
        
        public string[] GetTheStockDatesLongest()
        {
            // return the date of longest stock to
            // be used as labels of portfolio performance 
            // chart. Portfolio timeseries requires longest data
            // of all stock added in portfolio 
            int LongestIndex = 0;

            int Longest = StockDates[0].Length;

            for (int i = 0; i < StockDates.Count; i++)
            {

                if (Longest < StockDates[i].Length)
                {
                    Longest = StockDates[i].Length;
                    LongestIndex = i;
                }
            }

            if (StockDates.Count > 0)
            {
               
                return StockDates[LongestIndex];
            }
            else
            {
                return null;
            }
        }

        public void CreateRoRsForPort(List<double[]> list)
        {
            // average rate of return of stock in portfolio
            double[] tempROR;

            if (list.Count > 0)
            {
                tempROR = new double[list.Count];

                for (int i = 0; i < tempROR.Length; i++)
                {
                    tempROR[i] = Queryable.Average(list[i].AsQueryable());

                    tempROR[i] = Math.Round(tempROR[i], 4);
                }

                RoRsOfStocksInport = tempROR;
            }

            else
            {
                RoRsOfStocksInport = null;
            }
        }

        public void CalculateVarAndSDs(List<double[]> list)
        {
            StockStatistics st = new StockStatistics();

            // individual variance and sd for stocks
            // in portfolio

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    InVariances.Add(Math.Round(st.GetVariance(list[i]),4));
                    InSDs.Add(Math.Round(st.GetSD(list[i]), 4));
                }
            }
        }
        
    }
}
