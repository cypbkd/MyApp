using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyApp.Model;
using MyApp.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewOrEditCoursePage : Page
    {
        public NewOrEditCoursePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CourseViewModel cvm = e.Parameter as CourseViewModel;
            this.DataContext = cvm;
        }

        private void SaveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CourseViewModel cvm = this.DataContext as CourseViewModel;            
            if (cvm.Course == null)
            {
                Course course = new Course();
                course.LessonId = cvm.LessonId;
                course.Location = location.Text;
                course.Name = courseName.Text;
                course.TeacherName = teacherName.Text;
                course.Remark = remark.Text;
                course.Dates = new List<DateTime>();

                DateTime dt = Params.FirstDay;
                int start = Convert.ToInt16(startWeek.Value);
                int end = Convert.ToInt16(endWeek.Value);
                if (end > Params.WeekCount) end = Params.WeekCount;

                while (dt.DayOfWeek != DayOfWeek.Monday)
                {
                    dt = dt.AddDays(-1);
                }

                int head = 1;
                while (head <= end)
                {
                    if (dt.DayOfWeek == cvm.DayOfWeek)
                    {
                        if (head >= start && everyWeek.IsChecked == true || (evenWeek.IsChecked == true && head % 2 == 0) || (oddWeek.IsChecked == true && head % 2 == 1))
                        {
                            course.Dates.Add(new DateTime(dt.Ticks));
                        }
                        head++;
                    }
                    dt = dt.AddDays(1);
                }
                App.CourseStore.Add(course);
            }

            App.Save();
            Frame.GoBack();
        }

        private void CancleButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //var frame = new Frame();
            //var content = new MainPage();
            //frame.Content = content;
            //Window.Current.Content = content;
            //Window.Current.Activate();
            Frame.GoBack();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            CourseViewModel cvm = this.DataContext as CourseViewModel;
            if (cvm.Course != null)
            {
                var cr = App.CourseStore.Where(c => c.Id == cvm.Course.Id).FirstOrDefault();

                if (cr != null)
                {
                    App.CourseStore.Remove(cr);
                }
            }

            Frame.GoBack();
        }
    }
}
