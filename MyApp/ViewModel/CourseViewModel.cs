using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Model;
using Windows.UI;

namespace MyApp.ViewModel
{
    public class CourseViewModel : INotifyPropertyChanged
    {
        private Course _course;
        public Course Course
        {
            get { return _course; }
            set
            {
                _course = value;
                RaisePropertyChanged("Course");
            }
        }

        public string Title
        {
            get
            {
                if (Course == null) return "新建";
                else return "编辑";
            }
        }

        public Boolean IsVisible
        {
            get
            {
                if (Course == null) return true;
                else return false;
            }
        }

        public Boolean IsCollapsed
        {
            get
            {
                if (Course == null) return false;
                else return true;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                if (Course == null) return Color.FromArgb(0xFF, 0xDE, 0xDE, 0xDE);
                else return Color.FromArgb(0xFF, 0xCE, 0xCE, 0xCE);
            }     
        }

        // For Edit
        public Guid LessonId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int WeekCount { get { return Params.WeekCount; } }
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
