using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using MyApp.Model;

namespace MyApp.DataManager
{
    public class DayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;

            return String.Format("{0}/{1}/{2}", dt.Year, dt.Month, dt.Day);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            String str = (String)value;

            //string[] result = str.Split(new Char[] {'/', ' '});

            var strs = str.Split('/', ' ');
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            DateTime dt;

            try
            {
                int.TryParse(strs[0], out year);
                int.TryParse(strs[1], out month);
                int.TryParse(strs[2], out day);

                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                day = DateTime.Now.Day;
            }
            catch
            {
            }
            finally
            {
                try
                {
                    dt = new DateTime(year, month, day);
                }
                catch
                {
                    dt = DateTime.Now.SubYYMMDD();
                }
            }

            return dt;
        }
    }
}
