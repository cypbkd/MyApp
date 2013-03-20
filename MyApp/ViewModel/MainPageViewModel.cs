using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Model;

namespace MyApp.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged 
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dt">周一的日期</param>
        public MainPageViewModel(DateTime dt)
        {
            Courses = new ObservableCollection<CourseViewModel>();
            Lessons = new ObservableCollection<Lesson>();
            Days = new ObservableCollection<Day>();

            // 添加天           
            foreach (var d in ModelHelper.getDays(dt))
            {
                Days.Add(d);
            }
            // 添加课程
            foreach (var l in ModelHelper.getLessons())
            {
                Lessons.Add(l);
            }

            foreach (var c in ModelHelper.getWeekCourses(dt))
            {
                Courses.Add(c);
            }

            int WeekNum = 1;

            DateTime dtt = Params.FirstDay;
            double split = (dt - dtt).TotalDays / 7;

            WeekNum = (int)Math.Ceiling(split);
            if (WeekNum <= 0) Title = "未开学";
            else if (WeekNum > Params.WeekCount) Title = "放假了";
            else
            {
                Title = String.Format("第{0}周", WeekNum);
            }

            GridHeight = Params.LessonCount * 102;
        }

        public ObservableCollection<CourseViewModel> Courses { get; set; }
        public ObservableCollection<Lesson> Lessons { get; set; }
        public ObservableCollection<Day> Days { get; set; }


        private double _gridHeight;
        public double GridHeight
        {
            get { return _gridHeight; }
            set
            {
                _gridHeight = value;
                RaisePropertyChanged("GridHeight");
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
            }
        }

        public String Today
        {
            get
            {
                var today = DateTime.Now.SubYYMMDD();
                return String.Format("星期{0}({1}月{2}日)", ((int)today.Date.DayOfWeek).ToChineseWeekDayStr(), today.Month, today.Day);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
