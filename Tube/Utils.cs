using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glue
{
    /// <summary>
    /// 
    /// TODO GET RID OF THIS CLASS
    /// 
    /// Dumping ground for random stuff. 
    /// 
    /// "There is a fine line between a clearing-house and a dumping-ground"
    /// 
    /// </summary>
    class Utils
    {
        internal static string FormatSeparatedList<T>(List<T> list, string separator)
        {
            string formattedList = "";
            int itemCount = 0;
            foreach (T item in list)
            {
                formattedList += item.ToString();
                if (++itemCount < list.Count)
                {
                    formattedList += ", ";
                }
            }

            return formattedList;
        }
    }
}
