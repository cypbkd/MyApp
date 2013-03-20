using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;

namespace MyApp.DataManager
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;

            return String.Format("{0:00}:{1:00}",dt.Hour,dt.Minute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            String str = (String)value;

            var strs = str.Split(':');
            int hour = 8;
            int minite = 20; 
            DateTime dt;

            try
            {
                int.TryParse(strs[0], out hour);
                int.TryParse(strs[1], out minite);

                hour %= 24;
                minite %= 60;
            }
            catch
            {
            }
            finally
            {
                dt = new DateTime(1, 1, 1, hour, minite, 0);
            }
            return dt;
        }
    }
}
