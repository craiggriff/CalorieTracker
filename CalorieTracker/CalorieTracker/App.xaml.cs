using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CalorieTracker
{
    public partial class App : Application
    {
        private static App app;

        public bool b_test = false;
        public static App GetApp
        {
            get { return app; }
        }

        public static ISaveAndLoad fileService;

        public static ISaveAndLoad Files
        {
            get { return fileService; }
        }

        private static CalorieTrackerDatabase _data;

        public static CalorieTrackerDatabase Database
        {
            get
            {
                if (_data == null)
                {
                    _data = new CalorieTrackerDatabase(Files.AddPathToFilename("CalorieTrackerSQLite.db3"));
                }
                return _data;
            }
        }

        

        public App()
        {
            InitializeComponent();
            app = this;

            fileService = fileService = DependencyService.Get<ISaveAndLoad>();
            fileService.CreateDirectory("");
            fileService.CreateDirectory("Photos");

            //MainPage = new MainPage();
            MainPage = new NavigationPage(new PortionList());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
