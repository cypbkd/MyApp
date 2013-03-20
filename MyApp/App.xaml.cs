using System;
using System.Collections.Generic;
using MyApp.Model;
using MyApp.DataManager;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using CharmFlyoutLibrary;
using CharmDemoGridApp;
using CharmDemoGridApp.Common;
using System.Linq;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace MyApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        ///
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            //初始化设置
            if (localSettings.Values.ContainsKey("fs"))
            {
                Params.FirstSet = (bool)localSettings.Values["fs"];
            }
            else
            {
                Params.FirstSet = true;
            }

            if (localSettings.Values.ContainsKey("fd"))
            {
                Params.FirstDay = DateTime.Parse(localSettings.Values["fd"].ToString());
            }
            else
            {
                Params.FirstDay = new DateTime(2012, 9, 3);
                localSettings.Values["fd"] = Params.FirstDay.ToString();
            }

            if (localSettings.Values.ContainsKey("wc"))
            {
                Params.WeekCount = int.Parse(localSettings.Values["wc"].ToString());
            }
            else
            {
                Params.WeekCount = 20;
                localSettings.Values["wc"] = Params.WeekCount;
            }

            if (localSettings.Values.ContainsKey("lc"))
            {
                Params.LessonCount = int.Parse(localSettings.Values["lc"].ToString());
            }
            else
            {
                Params.LessonCount = 8;
                localSettings.Values["lc"] = Params.LessonCount;
            }

            Read();
        }

        //添加隐私条款
        void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            UICommandInvokedHandler handler = new UICommandInvokedHandler(CommandHandler);
            SettingsCommand setting = new SettingsCommand("Privacy", "隐私条款", handler);
            args.Request.ApplicationCommands.Add(setting);
        }
        private async void CommandHandler(IUICommand command)
        {
            if (command.Id == "Privacy")
                await Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.cnblogs.com/fallfine/articles/2704769.html", UriKind.Absolute));
        }

        //初始化通知和磁贴
        private void InitialTilesAndToasts()
        {
            //清空Tile队列
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            int TileCount = 0;
            foreach(var i in new List<Int16>{0,1,2})
            {
                var now = DateTime.Now.AddDays(i);
                var lst = ModelHelper.getDayCourse(now);
                foreach (var course in lst)
                {
                    //计算每节课的开始时间和结束时间
                    Lesson lesson = LessonStore.Where(l => l.Id == course.LessonId).FirstOrDefault();
                    lesson.StartTime = new DateTime(now.Year, now.Month, now.Day, lesson.StartTime.Hour, lesson.StartTime.Minute, lesson.StartTime.Second);
                    lesson.EndTime = new DateTime(now.Year, now.Month, now.Day, lesson.EndTime.Hour, lesson.EndTime.Minute, lesson.EndTime.Second);
                    DateTimeToStringConverter dtc = new DateTimeToStringConverter();
                    String lessonString = dtc.Convert(lesson.StartTime, null, null, null).ToString() + " - " + dtc.Convert(lesson.EndTime, null, null, null).ToString();

                    if (TileCount < 5)
                    {
                        //计划Tiles
                        XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideBlockAndText01);
                        XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
                        tileTextAttributes[0].InnerText = course.Name;
                        tileTextAttributes[1].InnerText = course.Location;
                        tileTextAttributes[2].InnerText = lessonString;
                        tileTextAttributes[4].InnerText = DateTime.Now.Day.ToString();
                        tileTextAttributes[5].InnerText = String.Format("星期{0}", ((int)DateTime.Now.DayOfWeek).ToChineseWeekDayStr());

                        // create the square template and attach it to the wide template
                        XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText01);
                        XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
                        squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode(course.Name));
                        squareTileTextAttributes[1].AppendChild(squareTileXml.CreateTextNode(course.Location));
                        squareTileTextAttributes[2].AppendChild(squareTileXml.CreateTextNode(lessonString));

                        IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
                        tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

                        if (lesson.StartTime > DateTime.Now && i == 0)
                        {
                            ScheduledTileNotification scheduledTile = new ScheduledTileNotification(tileXml, DateTime.Now.AddSeconds(10));
                            TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(scheduledTile);
                            TileCount++;
                        }
                    }

                    //计划Toast
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastText04;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode("下节课：" + course.Name));
                    toastTextElements[1].AppendChild(toastXml.CreateTextNode("地点：" + course.Location));
                    toastTextElements[2].AppendChild(toastXml.CreateTextNode("时间：" + lessonString));

                    if (lesson.StartTime > DateTime.Now)
                    {
                        int baseV = 5;
                        int mul = 2;
                        while (lesson.StartTime.AddMinutes(baseV * -1) > now && baseV <= 80)
                        {
                            ScheduledToastNotification scheduledToast = new ScheduledToastNotification(toastXml, lesson.StartTime.AddMinutes(baseV * -1));
                            ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
                            baseV *= mul;
                        }
                    }

                }
            }
        }
        object obj = new object();

        public static MyCharmFlyouts mcf;
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            // Create a Frame to act navigation context and navigate to the first page
            //添加设置页面
            mcf = new MyCharmFlyouts();
            var rootFrame = new CharmFrame { CharmContent = mcf };
            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
            if (!rootFrame.Navigate(typeof(MainPage)))
            {
                throw new Exception("Failed to create initial page");
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();

            //欢迎界面
            if (Params.FirstSet)
            {
                var messageDialog = new MessageDialog("欢迎使用大学课程表，请自定义您的个人设置。", "提示");

                // Add commands and set their callbacks
                messageDialog.Commands.Add(new UICommand("确定", (command) =>
                {
                    Params.FirstSet = false;
                    localSettings.Values["fs"] = Params.FirstSet;

                    mcf.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }));

                messageDialog.ShowAsync();
            }

            SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
            InitialTilesAndToasts();
            Save();
        }


        private static List<Course> _courseStore;
        public static List<Course> CourseStore
        {
            get 
            {            
                return _courseStore;
            }
            set { _courseStore = value; }
        }

        private static List<Lesson> _lessonStore;
        public static List<Lesson> LessonStore
        {
            get
            {                
                return _lessonStore;
            }
            set { _lessonStore = value; }
        }


        public async static void Save()
        {
            String courseStr = DataOperation.JsonSerialize(_courseStore);
            String lessonStr = DataOperation.JsonSerialize(_lessonStore);
            await DataOperation.SaveAsync<String>(courseStr, "course.json").ConfigureAwait(false);
            await DataOperation.SaveAsync<String>(lessonStr, "lesson.json").ConfigureAwait(false);
        }

        public async static void Read()
        {
            String courseStr = await DataOperation.RestoreAsync<String>("course.json").ConfigureAwait(false);
            String lessonStr = await DataOperation.RestoreAsync<String>("lesson.json").ConfigureAwait(false);            

            if (!String.IsNullOrEmpty(courseStr))
            {
                _courseStore = DataOperation.JsonDeserialize<List<Course>>(courseStr);
            }
            if (_courseStore == null)
            {
                _courseStore = new List<Course>();
            }
            if (!String.IsNullOrEmpty(lessonStr))
            {
                _lessonStore = DataOperation.JsonDeserialize<List<Lesson>>(lessonStr);
            }
            if (_lessonStore == null)
            {
                InitialLesson();
            }
        }

        public static void InitialLesson()
        {
            _lessonStore = new List<Lesson>();

            _lessonStore.Add(new Lesson()
            {
                Num = 1,
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 8, 45, 0),
            });

            for (int i = 1; i < Params.LessonCount; i++)
            {
                if (i % 2 == 0)
                {
                    _lessonStore.Add(new Lesson()
                    {
                        Num = i + 1,
                        StartTime = _lessonStore[i - 1].EndTime.AddMinutes(15),
                        EndTime = _lessonStore[i - 1].EndTime.AddMinutes(60),
                    });
                }
                else
                {
                    _lessonStore.Add(new Lesson()
                    {
                        Num = i + 1,
                        StartTime = _lessonStore[i - 1].EndTime.AddMinutes(5),
                        EndTime = _lessonStore[i - 1].EndTime.AddMinutes(50),
                    });
                }
            }

            Save();
        }
    }
}
