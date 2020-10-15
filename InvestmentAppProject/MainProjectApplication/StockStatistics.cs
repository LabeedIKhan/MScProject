using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MainProjectApplication
{
    public class StockStatistics
    {
        /* Class Description 
         * This class function is to provide calculations
         * to all part of the program where needed though
         * most clauclations are given to Stock object
         * DisplayManager create Rate of return array for
         * all data that is obtained from api to show in 
         * numerical values table using RateOfReturn method
         * of this class. The class uses System.Math class
         * and System.Linq.Queryable 
         * of c# and do some basic statistics. 
         * Math.Round() method set limits to the numbers
         * after decimal so they don't become to large
         *
         * Testing for these calculation has been done using 
         * excel by giving same values to method below and to 
         * excel formulas to see if they are correct
         * All test has been passed test class StockStatisitcsTest.cs
         * 
         * Rate of Return is the amount earned
         * by seller in this case the stock sold
         * on any particular date shown in numerical 
         * values table in stocks class, 
         * calulation formula is taken from 
         * corporatefinanceinstutute.com 
         * rateofreturn = (newvalue - oldvalue)/ oldvalue
         * https://corporatefinanceinstitute.com/resources/knowledge/finance/rate-of-return-guide/
         * and multiply by 100 if needed as percentage 
         */

        public StockStatistics()
        {

        }

        public double GetMaximum(double[] numbers)
        {
            double max = 0.0;
            for (int i = 0; i < numbers.Length; i++)
            {
                if (max < numbers[i])
                {
                    max = numbers[i];
                }
            }

            return max;
        }

        public double GetMinimum(double[] numbers)
        {
            double min = numbers[0];
            for (int i = 0; i < numbers.Length; i++)
            {
                if (min > numbers[i])
                {
                    min = numbers[i];
                }
            }
            return min;
        }

        public double GetAverage(double[] numbers)
        {
            double average = Queryable.Average(numbers.AsQueryable());
            return Math.Round(average, 4);
        }

        // calculate variance of Rate of Return array
        public double GetVariance(double[] number)
        {
            double ave = number.Average();
            double forSD = number.Select(val => (val - ave) * (val - ave)).Sum();
            return Math.Round(forSD / number.Length, 5);
        }
        // sqrt of variance 
        public double GetSD(double[] number)
        {
            double ave = number.Average();
            double forSD = number.Select(val => (val - ave) * (val - ave)).Sum();
            double SD = Math.Sqrt(forSD / number.Length);
            return Math.Round(SD, 5);
        }

        public double GetRange(double[] number)
        {
            double Range = GetMaximum(number) - GetMinimum(number);
            return Range;
        }

        //========Rate of Return=============
        public double[] RateOfReturn(double[] number)
        {
            // formula information given in description above
            double RoR = 0.0;
            double[] RoRSeries = new double[number.Length - 1];

            for (int i = 0; i < number.Length - 1; i++)
            {
                RoR = (number[i] - number[i + 1]) / number[i + 1];

                RoRSeries[i] = Math.Round(RoR, 4);
            }
            return RoRSeries;
        }
    }
}
