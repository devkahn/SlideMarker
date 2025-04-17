using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Helpers
{
    public static class DateHelper
    {
        public static string ConvertDisplayData(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }
        public static DateTime ConvertFromTimeStamp(string stamp)
        {
            DateTime output = new DateTime();

            long stampLong = long.Parse(stamp);

            output = output.AddMilliseconds(stampLong);

            return output;
        }
    }
}
