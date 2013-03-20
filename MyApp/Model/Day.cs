using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace MyApp.Model
{
    public class Day
    {
        public DateTime Date { get; set; }

        public String DayOfWeekName { get; set; }

        public Color BackColor
        {
            get
            {
                if (Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    return Color.FromArgb(0xFF, 0xBB, 0xBB, 0xBB);
                }
                else
                {
                    return Color.FromArgb(0xFF, 0xEE, 0xEE, 0xEE);
                }
            }
        }
    }
}
