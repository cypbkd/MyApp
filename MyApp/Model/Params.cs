using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Model
{
    // weekCnt  周数
    // headDay  第一周是哪一天
    // classCnt 每天几节课
    public class Params
    {
        public static DateTime FirstDay { get; set; }

        public static int WeekCount { get; set; }

        public static int LessonCount { get; set; }

        public static bool FirstSet { get; set; }
    }
}
