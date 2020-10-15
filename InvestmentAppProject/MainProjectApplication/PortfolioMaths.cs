using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace MainProjectApplication
{
    /*  The following class consists of method to calculate 
     *  mathmatical values regarding portfolio by converting
     *  portfolio formulas into c# alogorythms the formulas used
     *  are from Corporate Finanace Institute CFI 
     *  retirieved from 
     *  https://corporatefinanceinstitute.com/resources/knowledge/finance/portfolio-variance/
     *  formulas are 
     *  portfolio variance = = w1^2  σ1^2 + w2^2  σ2^2 + 2 w1w2σ1σ2Cov 1,2
     *  weight of stock = =  asset1/ total value of all assets
     *  porfolio standard deviation = sqrt(variance)
     *  covariance calculation is 
     *  cov = (x - mean of x)(y - mean of y)/n
     *  from Corporate fianance Institute 
     *  https://corporatefinanceinstitute.com/resources/knowledge/finance/covariance/
     *  correlation can calculated as 
     *  corr = (x - mean of x)(y - mean of y)/ sqrt((x - mean of x)^2(y - mean of y)^2)
     *  from corporate finance institute 
     *  https://corporatefinanceinstitute.com/resources/knowledge/finance/correlation/
     *  for above claculation to work program 
     *  needs to create a coorellation and covariance matrix 
     *  as well just create matrix of all asset with all other assets 
     *  https://corporatefinanceinstitute.com/resources/excel/study/correlation-matrix/
     *  another covariacne table is also needed 
     *  Method of calculating expected portfolio 
     *  return is ER = WA * RA + WB * RB so on
     *  depending on number of returns 
     *  formalu have been taken from Investopedia 
     *  https://www.investopedia.com/ask/answers/061215/how-can-i-calculate-expected-return-my-portfolio.asp
     */


    public class PortfolioMaths : StockStatistics
    {

        public PortfolioMaths()
        {

        }

        public double[,] CovMatrix(double[][] matrix, int len)
        {
            // calculate Covariance matrix among all asset
            // using GetCovariacne method that implement 
            // covariance formula 

            double[,] covar = new double[len, len];
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    covar[i, j] = GetCovariance(matrix[i], matrix[j]);

                }

            }
            return covar;
        }

        public double[,] CorrMatrix(double[][] matrix, int len)
        {
            // calculate Correlation matrix among all asset
            // using GetCorrelation method that implement 
            // correlation formula 
            double[,] corr = new double[len, len];

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    corr[i, j] = GetCorrelation(matrix[i], matrix[j]);

                }

            }
            return corr;
        }
        public double GetCovariance(double[] number1, double[] number2)
        {
            // Implement covariance formula described in
            // above description 
            double cov = 0.0;
            for (int i = 0; i < number1.Length; i++)
            {
                //(x - mean of x)(y - mean of y)
                cov += (number1[i] - GetAverage(number1)) * (number2[i] - GetAverage(number2));
            }
            //(x - mean of x)(y - mean of y)/n
            return Math.Round(cov / number1.Length, 5);
            // The method has been tested using 
            // excel inbuilt covariacne formula 
            // in PortfolioMathTest.cs
        }

        

        public double GetCorrelation(double[] number1, double[] number2)
        {
           /* This method implement correlation formula 
            * explanied in description, The method has been 
            * tested for accuracy of calculation using excel
            * built in correlation formula in PortfolioMathTest.cs
            * (x - mean of x)(y - mean of y)/ sqrt((x - mean of x) ^ 2(y - mean of y) ^ 2)
            */
            double summul = 0.0, sqrsum1 = 0.0, sqrsum2 = 0.0;
            double[] submean1 = new double[number1.Length], submean2 = new double[number2.Length];
            double[] sqrmean1 = new double[number1.Length], sqrmean2 = new double[number2.Length];
            double[] mulsub = new double[number1.Length];
            double avenum1 = GetAverage(number1), avenum2 = GetAverage(number2);

            for (int i = 0; i < number1.Length; i++)
            {
                //(x - mean of x)
                submean1[i] = number1[i] - avenum1;
                //(y - mean of y)
                submean2[i] = number2[i] - avenum2;
                // (x - mean of x)(y - mean of y)
                mulsub[i] = submean1[i] * submean2[i];
                //(x - mean of x) ^ 2
                sqrmean1[i] = Math.Pow(submean1[i], 2);
                // (y - mean of y) ^ 2
                sqrmean2[i] = Math.Pow(submean2[i], 2);

                summul += mulsub[i];
                sqrsum1 += sqrmean1[i];
                sqrsum2 += sqrmean2[i];
            }
            // main formula 
            return Math.Round(summul / Math.Sqrt(sqrsum1 * sqrsum2), 5);
        }
       

        public double PortfolioEr(double[][] stocks, double[] weighting)
        {
            /* This method calculate both eqaul weights
             * and adjusted weights, using formula explained 
             * is description comments at start
             * for equal weight no 
             * weight will be given and it called from 
             * Portfolio.cs main setter method. 
             */
            double[] stock = new double[] { };
            double[] weight = new double[stocks.GetLength(0)];
            double portfoliosum = 0.0;
            // if no weights given 
            if (weighting == null)
                for (int i = 0; i < stocks.GetLength(0); i++)
                {
                    weight[i] = (1.0 * 1 / stocks.GetLength(0));

                }
            else
            {
                // if weights given
                weight = weighting;
            }
            // more information about fromula in description
            // ER = WA * RA + WB * RB so on
            for (int i = 0; i < stocks.GetLength(0); i++)
            {
                stock = stocks[i].ToArray();
                // stock are already in returns
                double[] ror = stock;
                // getaverage for stock * with weight 
                portfoliosum += GetAverage(ror) * weight[i];


            }
            return Math.Round(portfoliosum, 4);

        }

        public double PortfolioVariance(double[][] stocks, double[] weighting, double[,] CovMatrix)
        {
            /* This method calculates portfolio variacne using 
             * portfolio variance formula explained above
             * w1^2  σ1^2 + w2^2  σ2^2 + 2 w1w2Cov 1,2
             * portfolio can be equal weight or adjusted 
             * weight if equal weight porgram will assign 
             * equal weight to all stocks.
             */

            double variance = 0;

            double[] weight = new double[stocks.GetLength(0)];

            // double weight = (1.0 * 1 / stocks.GetLength(0));

            // if weight is null assign equal weights
            if (weighting == null)
                for (int i = 0; i < stocks.GetLength(0); i++)
                {
                    weight[i] = Math.Round((1.0 * 1 / stocks.GetLength(0)), 6);
                     
                }
                else
                {
                // given weights
                weight = weighting;
                }
            // for individual variances 
            double[] invariance = new double[stocks.GetLength(0)];

            for (int i = 0; i < invariance.Length; i++)
            {
                // calculate individual variance of all stocks
                double[] RateOfReturn = stocks[i];
                invariance[i] = GetVariance(RateOfReturn);
            }

            double number = 0.0;

            for (int i = 0; i < invariance.Length; i++)
            {
                // first part of formula loop
                // depedning on number of stocks 
                // w1^2  σ1^2 + w2^2  σ2^2
                number += (weight[i] * weight[i]) * (invariance[i] * invariance[i]);
            }

            for (int i = 0; i < CovMatrix.GetLength(0)-1; i++)
            {
                for (int j = 0 + i; j < CovMatrix.GetLength(1)-1; j++)
                {
                    // 2 w1w2σ1σ2Cov 1,2
                    // loop over all stock and combination 
                    // loop over cov matrix 
                    // Corrlation matrix is explained 
                    // above but this formula require covariance matrix
                    number += 2 * weight[j] * weight[j + 1] * invariance[j] * invariance[j + 1] * CovMatrix[j, j + 1];
                }
            }
            //variance = Math.Sqrt(number);
            variance = number;
            
            return variance;
        }

        public double PortfolioSD(double num)
        {
            // as described above portfolio sd is sqrt(variance) of portfolio
            return Math.Round(Math.Sqrt(num), 4);
        }

        public double[] GetWeights(double[][] stocks)//double[,] CovMatrix
        {
            /* Provide weight for indivudual stock
             * in porfolio using formula described 
             * at start of file. 
             * stock/value of all stocks
             * It is not known what value a stock
             * would have so the best would be average
             * all values. 
             */
            double[] stockList = new double[stocks.Length];

            for (int i = 0; i < stockList.Length; i++)
            {
                // value of stock here as
                // average of all performance 
                stockList[i] = Queryable.Average(stocks[i].AsQueryable());
            }

            double total = 0.0;

            for (int i = 0; i < stockList.Length; i++)
            {
                // value of portfolio
                total += stockList[i];
            }

            double[] weights = new double[stockList.Length];
            // get weight for each stock
            for (int i = 0; i < weights.Length; i++)
            {
                double num = stockList[i] * 100;

                weights[i] = 1.0 * num / total;

                // there cannot be negative weight
                // if calculations go below zero
                // weight would be zero
                if (weights[i] < 0)
                {
                    weights[i] = 0.0;
                }
                if (weights[i] > 100)
                {
                    weights[i] = 100.0;
                }
            }
            return weights;
        }
        
        public double[][] ValidFormat(double[][] arrays)
        {
            /* This method creates equal length
             * arrays for all stock by changing missing 
             * values with their present values averages
             * the length is determined by longest array
             * and remining arrays will be adjusted to 
             * longest array. More detials about missing
             * and their replacmentare in main report 
             * System Architecture Design part. 
             */

            int len = 0;

            double[][] format = new double[arrays.Length][];

            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i].Length > len)
                {
                    // takes longest length
                    len = arrays[i].Length;
                }
            }
            for (int i = 0; i < arrays.Length; i++)
            {
                // every array has longest length
                format[i] = new double[len];
                for (int j = 0; j < format[i].Length; j++)
                {
                    // add all averages 
                    format[i][j] = GetAverage(arrays[i]);
                }
            }
            for (int i = 0; i < arrays.Length; i++)
            {
                for (int j = 0; j < arrays[i].Length; j++)
                {
                    // where values present will replace
                    // averages with real values 
                    // if missing, the values will be average
                    format[i][j] = arrays[i][j];

                }
            }
            // retun data in symetric length arrays 
            return format;
        }

        public double[] GetPortfolioTimeSeries(double[][] data)
        {
            /* This method merge perfromance of all
             * stock to create estimated performance of
             * Portfolio on a given date in past.
             * To create portfolio historic performance 
             * just like stock, no special formula
             * used just average of all stocks at given 
             * date. 
             */

            double[] timeseries = new double[data[0].Length];

            double[][] RoRArray = new double[data.Length][];

            for (int i = 0; i < data.Length; i++)
            {
                RoRArray[i] = data[i];
            }
            for (int i = 0; i < RoRArray[0].Length; i++)
            {
                double average = 0.0;
                double num = 0;
                for (int j = 0; j < RoRArray.Length; j++)
                {
                    // add all stocks performance
                    num += RoRArray[j][i];
                    // divide it with their numbers
                    average = num / RoRArray.Length;
                }
                timeseries[i] = Math.Round(average, 4);

            }
            return timeseries;
        }

        public string[,] MyMatrixTable(string[] symbols, double[,] matrix, string type)
        {
            /* Add label companies symbols
             * to the matrixes used above 
             * to be shown in advance tab 
             * of MainWindow. It is called
             * separately for both tables
             * from Portfolio class. 
             */

            int len = matrix.GetLength(0) + 1;

            string[,] mytable = new string[len, len];

            mytable[0, 0] = "Companies";

            for (int i = 1; i < len; i++)
            {
                mytable[0, i] = symbols[i - 1];
                mytable[i, 0] = symbols[i - 1];
            }
            // for covariacne matrix 
            if (type == "cov")
            {
                for (int i = 1; i < len; i++)
                {
                    for (int j = 1; j < len; j++)
                    {
                        matrix[i - 1, j - 1] = Math.Round(matrix[i - 1, j - 1], 3);
  
                        mytable[i, j] = matrix[i - 1, j - 1].ToString();

                    }
                }
            }
            // for correlation matrix 
            else
            {
                for (int i = 1; i < len; i++)
                {
                    for (int j = 1; j < len; j++)
                    {
                        matrix[i - 1, j - 1] = Math.Round(matrix[i - 1, j - 1], 3);

                        matrix[i - 1, j - 1] = matrix[i - 1, j - 1] * 100;
                        mytable[i, j] = matrix[i - 1, j - 1].ToString();

                        mytable[i, j] = mytable[i, j] + "%";
                    }
                }

            } 
            return mytable;
        }
    }
}
