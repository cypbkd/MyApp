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
    public class SnapViewModel : INotifyPropertyChanged 
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dt">周一的日期</param>
        public SnapViewModel(DateTime dt)
        {
            GridHeight = Params.LessonCount * 102;
            DayCourses = new ObservableCollection<CourseViewModel>();
            Lessons = new ObservableCollection<Lesson>();
            // 添加课程
            foreach (var l in ModelHelper.getLessons())
            {
                Lessons.Add(l);
            }

            foreach (var c in ModelHelper.getDayCourses(dt))
            {
                DayCourses.Add(c);
            }

            _today = String.Format("星期{0}({1}月{2}日)", ((int)dt.Date.DayOfWeek).ToChineseWeekDayStr(), dt.Month, dt.Day);
        }

        public ObservableCollection<CourseViewModel> DayCourses { get; set; }
        public ObservableCollection<Lesson> Lessons { get; set; }


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

        private String _today;
        public String Today
        {
            get { return _today; }
            set
            {
                _today = value;
                RaisePropertyChanged("Today");
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
