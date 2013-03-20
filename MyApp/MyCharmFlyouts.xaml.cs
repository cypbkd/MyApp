using System;
using MyApp;
using MyApp.DataManager;
using MyApp.Model;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CharmDemoGridApp
{
    public sealed partial class MyCharmFlyouts : UserControl
    {
        public ApplicationDataContainer localSettings;
        public EventHandler<object> DataChanged;
        public MyCharmFlyouts()
        {
            this.InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;

            Params settings = new Params();
            cfoSettings.DataContext = settings;

            SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequested;
        }

        private void CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("a", "关于", (p) => { cfoAbout.IsOpen = true; }));
            args.Request.ApplicationCommands.Add(new SettingsCommand("h", "帮助", (p) => { cfoHelp.IsOpen = true; }));
            args.Request.ApplicationCommands.Add(new SettingsCommand("s", "设置", (p) => { cfoSettings.IsOpen = true; }));
        }

        private async void OnMailTo(object sender, RoutedEventArgs args)
        {
            var hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                var uri = new Uri("mailto:cyp_bkd@qq.com");
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }

        //加载已保存的设置
        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Visibility = Windows.UI.Xaml.Visibility.Visible;

            Params settings = new Params();
            Params.FirstDay = DateTime.Parse(localSettings.Values["fd"].ToString());
            Params.WeekCount = int.Parse(localSettings.Values["wc"].ToString());
            Params.LessonCount = int.Parse(localSettings.Values["lc"].ToString());
            cfoSettings.DataContext = settings;
        }

        private async void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.CourseStore.Count == 0)
            {
                localSettings.Values["fd"] = Params.FirstDay.ToString();
                localSettings.Values["wc"] = Params.WeekCount;
                localSettings.Values["lc"] = Params.LessonCount;

                App.InitialLesson();
                DataChanged(sender, e);
            }

            if (App.CourseStore.Count > 0)
            {
                var messageDialog = new MessageDialog("更改很有可能影响已生成的课程表，你确定要更改？", "警告");

                // Add commands and set their callbacks
                messageDialog.Commands.Add(new UICommand("确定", (command) =>
                {
                    localSettings.Values["fd"] = Params.FirstDay.ToString();
                    localSettings.Values["wc"] = Params.WeekCount;
                    localSettings.Values["lc"] = Params.LessonCount;

                    App.InitialLesson();
                    DataChanged(sender,e);
                }));

                messageDialog.Commands.Add(new UICommand("取消", (command) =>
                {
                    Params.FirstDay = DateTime.Parse(localSettings.Values["fd"].ToString());
                    Params.WeekCount = int.Parse(localSettings.Values["wc"].ToString());
                    Params.LessonCount = int.Parse(localSettings.Values["lc"].ToString());
                }));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 1;

                // Show the message dialog
                await messageDialog.ShowAsync();
            }

            this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        //void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        //{
        //    var deferral = e.SuspendingOperation.GetDeferral();
        //    //TODO: Save application state and stop any background activity
        //    deferral.Complete();

        //    App.Save();
        //}

        private void CancleBtn_Click(object sender, RoutedEventArgs e)
        {
            Params.FirstDay = DateTime.Parse(localSettings.Values["fd"].ToString());
            Params.WeekCount = int.Parse(localSettings.Values["wc"].ToString());
            Params.LessonCount = int.Parse(localSettings.Values["lc"].ToString());

            this.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}
