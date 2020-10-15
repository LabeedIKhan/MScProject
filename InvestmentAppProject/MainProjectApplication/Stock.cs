using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProjectApplication
{
    public class Stock
    {
        /* This class provide stock object reference to
         * this bject is then passed to other part
         * of program by the DisplayManager class 
         * that create it, a new stock object is created
         * every time user select a stock and presses
         * statitistics to see that stock statisitcs 
         * the class uses other class StockStatistics 
         * to get all required calculation
         */

        private readonly string[] valnames = { "Average", "Maximum", "Minimum", "Range", "Variance", "Stand.Dev" };

        private string symbol;
        private string name;
        private double average;
        private double maximum;
        private double minimum;
        private double range;
        private double variance;
        private double standerdDeviation;

        /* The constructer is use to create 
         * stock object and it is called from
         * DisplayManager method GenerateStock
         * which passes symbol and name of stock
         * when it create stock object.
         */

        public Stock(string symbol, string name)
        {
            this.symbol = symbol;
            this.name = name;
        }

        //=======Getters for program
         public string TheStockSymbol { get => symbol; }
         public string TheStockName { get => name; }
         public double Average { get => average; }
         public double Maximum { get => maximum; }
         public double Minimum { get => minimum; }
         public double Range { get => range; }
         public double Variance { get => variance; }
         public double StanderdDeviation { get => standerdDeviation; }

        
         /* Once stock object has been created 
          * by GenerateStock method of DisplayManager
          * it passes numeric valuse to this
          * method which is rate of return calculated
          * and showed in numerical table.
          * There is only one setter, as it will gerenate
          * all calculations, which can then be called 
          * as needed through getters. 
          */

         public void CalculateStatistics(double[] val)
         {
              StockStatistics stockscal = new StockStatistics();
              average = stockscal.GetAverage(val);
              maximum = stockscal.GetMaximum(val);
              minimum = stockscal.GetMinimum(val);
              range = stockscal.GetRange(val);
              variance = stockscal.GetVariance(val);
              standerdDeviation = stockscal.GetSD(val);
         }

        /* Although getters are present sometime 
         * full object might be needed to show 
         * all values.
         */

         public override string ToString()
         {
          return  "Company Symbol: "   + symbol + "\n" +
                "Company Name: " + name + "\n"
                + valnames[0] + ": " + Math.Round(Average, 5) + "  "
                + valnames[1] + ": " + Maximum + "  "
                + valnames[2] + ": " + Minimum + "  "
                + valnames[3] + ": " + Range + "  "
                + valnames[4] + ": " + Math.Round(Variance, 5) + "  "
                + valnames[5] + ": " + Math.Round(StanderdDeviation, 5) + "  ";  
         }
        
    }
}
