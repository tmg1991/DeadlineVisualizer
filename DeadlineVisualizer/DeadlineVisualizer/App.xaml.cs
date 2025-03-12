namespace DeadlineVisualizer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitPreferences();
            MainPage = new AppShell();
        }

        private void InitPreferences()
        {
            SetDefaultDistance(Constants.warning_level_1_distance_key, Constants.warning_level_1_distance);
            SetDefaultDistance(Constants.warning_level_2_distance_key, Constants.warning_level_2_distance);
            SetDefaultDistance(Constants.warning_level_3_distance_key, Constants.warning_level_3_distance);
            if(!Preferences.Default.ContainsKey(Constants.default_calendar_unit))
            {
                Preferences.Default.Set(Constants.default_calendar_unit, CalendarUnits.CalendarWeeks.ToString());
            }

            void SetDefaultDistance(string key, int days){
                if (!Preferences.Default.ContainsKey(key))
                {
                    Preferences.Default.Set(key, days);
                }
            }

        }
    }
}
