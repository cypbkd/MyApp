using System;
using System.Linq;
using MyApp.Model;
using MyApp.ViewModel;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static DateTime NowDay = DateTime.Now;
        private static DateTime NowSnapDay = DateTime.Now;
        public ApplicationDataContainer localSettings;

        public MainPage()
        {
            this.InitializeComponent();

            //处理贴靠事件
            Window.Current.SizeChanged += (s, e) =>
            {
                var state = ApplicationView.Value;

                switch (state)
                {
                    case ApplicationViewState.Snapped:
                        VisualStateManager.GoToState(this, "Snapped", true);
                        break;
                    default:
                        VisualStateManager.GoToState(this, "HX", true);
                        break;
                }
            };

            //初始化设置和数据
            localSettings = ApplicationData.Current.LocalSettings;

            fv.DataContext = new MainPageViewModel(NowDay);
            lv.DataContext = new SnapViewModel(NowSnapDay);

            App.mcf.DataChanged += (o, e) => { BackToday(null, null); };
        }

        //新建或编辑课程
        private void NewOrEditCourse(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            CourseViewModel course = (sender as Border).DataContext as CourseViewModel;

            if (course == null)
            {
                return;
            }

            Frame.Navigate(typeof(NewOrEditCoursePage),course);
        }

        //设置本节课的时间段
        private void SetCourseTimeSpan(object sender, TappedRoutedEventArgs e)
        {
            if (!ParentedPopup.IsOpen) { ParentedPopup.IsOpen = true; }
            ParentedPopup.VerticalOffset = e.GetPosition(pageroot).Y;
            if (ParentedPopup.VerticalOffset > 560)
            {
                ParentedPopup.VerticalOffset = 560;
            }
            myPop.OK += (o, ee) =>
            {
                var mvm = fv.DataContext as MainPageViewModel;
                Lesson l = myPop.DataContext as Lesson;

                Lesson les = mvm.Lessons.Where(dl => dl.Id == l.Id).FirstOrDefault() ;

                int index = mvm.Lessons.IndexOf(les);
                mvm.Lessons.Remove(les);
                mvm.Lessons.Insert(index, l);

                App.Save();
            };

            myPop.DataContext = (sender as Border).DataContext;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            NowDay = NowDay.AddDays(-7);
            fv.DataContext = new MainPageViewModel(NowDay);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            NowDay = NowDay.AddDays(7);
            fv.DataContext = new MainPageViewModel(NowDay);
        }

        private void PreviousDayButton_Click(object sender, RoutedEventArgs e)
        {
            NowSnapDay = NowSnapDay.AddDays(-1);
            lv.DataContext = new SnapViewModel(NowSnapDay);
        }

        private void NextDayButton_Click(object sender, RoutedEventArgs e)
        {
            NowSnapDay = NowSnapDay.AddDays(1);
            lv.DataContext = new SnapViewModel(NowSnapDay);
        }

        //加载设置参数
        private void GotFocus(object sender, RoutedEventArgs e)
        {
            Params.FirstDay = DateTime.Parse(localSettings.Values["fd"].ToString());
            Params.WeekCount = int.Parse(localSettings.Values["wc"].ToString());
            Params.LessonCount = int.Parse(localSettings.Values["lc"].ToString());
        }

        private void BackToday(object sender, RoutedEventArgs e)
        {
            NowDay = DateTime.Now;
            NowSnapDay = DateTime.Now;
            fv.DataContext = new MainPageViewModel(NowDay);
            lv.DataContext = new SnapViewModel(NowSnapDay);
        }

        private void barShow(object sender, RightTappedRoutedEventArgs e)
        {
            btmBar.IsOpen = true;
        }
    }
}
