using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProjectApplication
{
    class Format
    {
        /* This class provides the functoinality of
         * two combo boxes on stock tab, The reason 
         * of having separate calss is DisplayManager
         * is getting too long and complicated,  
         * This class hasn't been unit tested because
         * result from action of comboboxes on profile 
         * table grid can be easily observed. 
         */


        public string[][] SortByPreferences(string[][] data, int col)
        {
            /* Called from DisplayManager.cs method 
             * SortByValue which tell this method 
             * on basis of what column the Profile table 
             * data needs to be sorted. it can be sort
             * by price, marketcap, PE/Ratio. 
             */
            Hashtable table = new Hashtable();
            
            string[][] sortData = new string[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                try
                {
                    // Enter to table with index and sortting value
                    table.Add(i, double.Parse(data[i][col]));
                }
                // if expection occurs don't sort it send originial 
                // data back. 
                catch (FormatException) { return data; }
            }

            int[] sortindex = new int[data.Length];

            // Sort hastable by value, not index 
            // convert it to list
            var sortted = table.Cast<DictionaryEntry>().OrderByDescending(entry => entry.Value).ToList();

            // loop over list to take index 
            // sorted by value of columns
            int counter = 0;
            foreach (var x in sortted)
            {
                // sortindex has index of data but sorted by value 
                // not index
                int val = int.Parse(x.Key.ToString());
                sortindex[counter] = val;
                counter++;
            }

            // create two array of array and place
            // arrays sorted by value so value become in decsending
            // order while index order is determined by value
            // and index would identify an array in array of array
            // so for example higher value in one column
            // will sort entire structure depending on it's descnding 
            // order
            for (int i = 0; i < sortindex.Length; i++)
            {
                int num = sortindex[i];
                sortData[i] = new string[data[num].Length];
                for (int j = 0; j < data[num].Length; j++)
                {
                    sortData[i][j] = data[num][j];
                }

            }
            return sortData;
        }

        public string[][] FilterByPreferences(string[][] data, string remove, int col)
        {
            /* Called from displaymanager class method
             * FilterByValue, row that contains particular
             * value in sepcified cloumn , for example 
             * remove all technology sector companies, 
             * sector column is col, and remove is value
             * on which row would be removed
             */

            // crate a new list
            List<string[]> list = new List<string[]>();

            // loop over sepcified column to see if value
            // matches value to be removed, if it does't 
            // match add it new list, so matching value
            // row would be excluded
            for (int i = 0; i < data.Length; i++)
            {

                if (data[i][col] != remove)
                {
                    list.Add(data[i]);
                }

            }

            // convert that list to array of array,
            string[][] newdata = new string[list.Count][];
            for (int i = 0; i < newdata.Length; i++)
            {
                newdata[i] = list[i].ToArray();
            }
            return newdata;
        }
        
    }
}
