using System.Windows.Input;

namespace DeadlineVisualizer
{
    public class SettingsPageViewModel : NotifyBase
    {
        private int _warningLevel1Distance;
        public int WarningLevel1Distance
        {
            get { return _warningLevel1Distance; }
            set
            {
                _warningLevel1Distance = value;
                ShouldShowSaveSuccess = false;
                NotifyPropertyChanged();
            }
        }

        private int _warningLevel2Distance;
        public int WarningLevel2Distance
        {
            get { return _warningLevel2Distance; }
            set
            {
                _warningLevel2Distance = value;
                ShouldShowSaveSuccess = false;
                NotifyPropertyChanged();
            }
        }

        private int _warningLevel3Distance;
        public int WarningLevel3Distance
        {
            get { return _warningLevel3Distance; }
            set
            {
                _warningLevel3Distance = value;
                ShouldShowSaveSuccess = false;
                NotifyPropertyChanged();
            }
        }

        private bool _autoUpdateCalendar;
        public bool AutoUpdateCalendar
        {
            get { return _autoUpdateCalendar; }
            set
            {
                _autoUpdateCalendar = value;
                ShouldShowSaveSuccess = false;
                NotifyPropertyChanged();
            }
        }

        private bool _shouldShowError;
        public bool ShouldShowError
        {
            get { return _shouldShowError; }
            set
            {
                _shouldShowError = value;
                NotifyPropertyChanged();
            }
        }

        private bool _shouldShowSaveSuccess;
        public bool ShouldShowSaveSuccess
        {
            get { return _shouldShowSaveSuccess; }
            set
            {
                _shouldShowSaveSuccess = value;
                NotifyPropertyChanged();
            }
        }
        public ICommand SaveCommand { get; private set; }

        private List<CalendarUnitDescription> _calendarUnitDescriptions = new();
        public List<CalendarUnitDescription> CalendarUnitDescriptions
        {
            get { return _calendarUnitDescriptions; }
            set
            {
                _calendarUnitDescriptions = value;
                NotifyPropertyChanged();
            }
        }

        private CalendarUnitDescription _selectedCalendarUnitDescription;
        public CalendarUnitDescription SelectedCalendarUnitDescription
        {
            get { return _selectedCalendarUnitDescription; }
            set
            {
                _selectedCalendarUnitDescription = value;
                NotifyPropertyChanged(); 
            }
        }

        public SettingsPageViewModel()
        {
            SaveCommand = new Command(Save);
            foreach (CalendarUnits unit in Enum.GetValues(typeof(CalendarUnits)))
            {
                CalendarUnitDescriptions.Add(new CalendarUnitDescription(unit));
            }
        }

        internal void OnAppearing()
        {            
            WarningLevel1Distance = Preferences.Default.Get(Constants.warning_level_1_distance_key, Constants.warning_level_1_distance);
            WarningLevel2Distance = Preferences.Default.Get(Constants.warning_level_2_distance_key, Constants.warning_level_2_distance);
            WarningLevel3Distance = Preferences.Default.Get(Constants.warning_level_3_distance_key, Constants.warning_level_3_distance);

            var unitFromSettings = Preferences.Default.Get(Constants.default_calendar_unit, CalendarUnits.CalendarWeeks.ToString());
            SelectedCalendarUnitDescription = CalendarUnitDescriptions.First(_ => _.Unit.ToString() == unitFromSettings);
            AutoUpdateCalendar = Preferences.Default.Get(Constants.auto_update_calendar, true);
        }
        private void Save()
        {
            ShouldShowSaveSuccess = false;
            if (AreDistancesValid())
            {
                ShouldShowError = false;
                Preferences.Default.Set(Constants.warning_level_1_distance_key, WarningLevel1Distance);
                Preferences.Default.Set(Constants.warning_level_2_distance_key, WarningLevel2Distance);
                Preferences.Default.Set(Constants.warning_level_3_distance_key, WarningLevel3Distance);

                Preferences.Default.Set(Constants.default_calendar_unit, SelectedCalendarUnitDescription.Unit.ToString());
                Preferences.Default.Set(Constants.auto_update_calendar, AutoUpdateCalendar);
                ShouldShowSaveSuccess = true;
                return;
            }
            ShouldShowError = true;
        }

        private bool AreDistancesValid() => _warningLevel3Distance > 0 &&
            _warningLevel2Distance > _warningLevel3Distance &&
            _warningLevel1Distance > _warningLevel2Distance;
    }
}
