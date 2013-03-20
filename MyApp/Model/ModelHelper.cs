using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.ViewModel;

namespace MyApp.Model
{
    public static class ModelHelper
    {
        /// <summary>
        /// 得到课程，从LessonStore中读取
        /// </summary>
        /// <returns></returns>
        public static List<Lesson> getLessons()
        {
            var less = App.LessonStore.OrderBy(l=>l.Num).ToList();

            return less;
        }

        internal static List<CourseViewModel> getWeekCourses(DateTime firstDay)
        {
            while (firstDay.DayOfWeek != DayOfWeek.Monday)
            {
                firstDay = firstDay.AddDays(-1);
            }
            var lst = new List<CourseViewModel>();
            for (int i = 0; i < 7; i++)
            {
                var daycourses = App.CourseStore.Where(c => c.Dates.Where(d => d <= firstDay.AddDays(i) && d.AddDays(1) >= firstDay.AddDays(i)).Count() > 0).ToList();
                //daycourses = daycourses.OrderBy(d => App.LessonStore.Where(l => l.Id == d.LessonId).First().Num).ToList();
                for (int j = 1; j < Params.LessonCount + 1; j++)
                {
                    Lesson lesson = App.LessonStore.Where(l => l.Num == j).FirstOrDefault();
                    if (lesson != null)
                    {
                        var course = daycourses.Where(d => d.LessonId == lesson.Id).FirstOrDefault();
                        lst.Add(
                            new CourseViewModel()
                            {
                                Course = course,
                                LessonId = lesson.Id,
                                DayOfWeek = firstDay.AddDays(i).DayOfWeek
                            });
                    }
                    else
                    {
                        lst.Add(null);
                    }
                }
            }
            return lst;
        }

        internal static List<CourseViewModel> getDayCourses(DateTime date)
        {
            var dt = new DateTime(date.Ticks);
            var lst = new List<CourseViewModel>();
            var daycourses = App.CourseStore.Where(c => c.Dates.Where(d => d <= dt.SubYYMMDD() && d.AddDays(1) >= dt.AddDays(1).SubYYMMDD()).Count() > 0).ToList();
            for (int j = 1; j < Params.LessonCount + 1; j++)
            {
                Lesson lesson = App.LessonStore.Where(l => l.Num == j).FirstOrDefault();
                if (lesson != null)
                {
                    var course = daycourses.Where(d => d.LessonId == lesson.Id).FirstOrDefault();
                    lst.Add(
                        new CourseViewModel()
                        {
                            Course = course,
                            LessonId = lesson.Id,
                            DayOfWeek = dt.DayOfWeek
                        });
                }
                else
                {
                    lst.Add(null);
                }
            }
            return lst;
        }

        internal static List<Course> getDayCourse(DateTime firstDay)
        {
            var lst = new List<Course>();
            var daycourses = App.CourseStore.Where(c => c.Dates.Where(d => d <= firstDay.SubYYMMDD() && d.AddDays(1) >= firstDay.AddDays(1).SubYYMMDD()).Count() > 0).ToList();
            for (int j = 1; j < Params.LessonCount + 1; j++)
            {
                Lesson lesson = App.LessonStore.Where(l => l.Num == j).FirstOrDefault();
                if (lesson != null)
                {
                    var course = daycourses.Where(d => d.LessonId == lesson.Id).FirstOrDefault();
                    if (course != null)
                    {
                        lst.Add(course);
                    }
                }
            }
            return lst;

        }

        public static List<Day> getDays(DateTime dt)
        {
            List<Day> days = new List<Day>();
            while (dt.DayOfWeek != DayOfWeek.Monday)
            {
               dt= dt.AddDays(-1);
            }
            for (int i = 0; i < 7; i++)
            {
                Day day = new Day();
                day.Date = dt.AddDays(i).ToLocalTime();

                day.DayOfWeekName = String.Format("星期{0}({1}月{2}日)", ((int)day.Date.DayOfWeek).ToChineseWeekDayStr()
                    , day.Date.Month, day.Date.Day);
                days.Add(day);
            }

            return days;
        }

        public static String ToChineseWeekDayStr(this int i)
        {
            if (i > 6) return "Error";

            if (i > 5) return "六";
            if (i > 4) return "五";
            if (i > 3) return "四";
            if (i > 2) return "三";
            if (i > 1) return "二";
            if (i > 0) return "一";

            return "日";
        }

        public static DateTime SubYYMMDD(this DateTime dt)
        {
            DateTime res = new DateTime(dt.Year,dt.Month,dt.Day,0,0,0);

            return res;
        }

        public static DateTime ToThisWeekMonday(this DateTime dt)
        {
            DateTime res = new DateTime(dt.Ticks);
            while (res.DayOfWeek != DayOfWeek.Monday)
            {
                res = res.AddDays(-1);
            }

            return res;
        }

    }
}
