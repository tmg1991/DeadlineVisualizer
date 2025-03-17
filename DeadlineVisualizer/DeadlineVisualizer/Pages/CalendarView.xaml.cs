using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace DeadlineVisualizer;

public partial class CalendarView : ContentView
{
    private const int MAX_COLUMNS = 15;
    private List<CalendarUnitDescription> calendarUnitDescriptions = new();

    public DateTime StartingDate { get; private set; }
    public CalendarUnitDescription SelectedCalendarUnitDescription
    {
        get => selectedCalendarUnitDescription;
        private set
        {
            selectedCalendarUnitDescription = value;
            Update(this, Milestones);
        }
    }

    public static readonly BindableProperty MilestonesProperty =
  BindableProperty.Create(nameof(Milestone), typeof(ObservableCollection<Milestone>), typeof(CalendarView), default(ObservableCollection<Milestone>), propertyChanged: OnMilestonesChanged);
    private CalendarUnitDescription selectedCalendarUnitDescription;

    public ObservableCollection<Milestone> Milestones
    {
        get => (ObservableCollection<Milestone>)GetValue(MilestonesProperty);
        set => SetValue(MilestonesProperty, value);
    }

    private static void OnMilestonesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CalendarView)bindable;

        var oldMilestones = oldValue as ObservableCollection<Milestone>;
        var newMilestones = newValue as ObservableCollection<Milestone>;

        if(oldMilestones != null)
        {
            oldMilestones.CollectionChanged -= NewMilestones_CollectionChanged;
        }
        if(newMilestones != null)
        {
            newMilestones.CollectionChanged += NewMilestones_CollectionChanged;
        }

        Update(view, newMilestones);

        void NewMilestones_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Update(view, newMilestones);
        }
    }

    private static void Update(CalendarView view, ObservableCollection<Milestone> milestones)
    {
        if(milestones == null)
        {
            return;
        }

        PrepareGrid(view.calendarGrid, milestones.Count);

        AddMilestonesToGrid(view.calendarGrid, milestones);

        CreateHeader(view);
        
        
    }

    private static void CreateHeader(CalendarView view)
    {
        for (int i = 0; i < MAX_COLUMNS; i++)
        {
            var date = view.StartingDate;
            var headerText = string.Empty;
            if(view.SelectedCalendarUnitDescription.Unit == CalendarUnits.CalendarWeeks)
            {
                date = date.AddDays(i*7);
                Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                int weekNumber = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                headerText = $"{date.Year}{Environment.NewLine}CW{weekNumber}";
            }
            else
            {
                date = date.AddDays(i);
                headerText = $"{date.Year}{Environment.NewLine}{date.ToString("MMM dd")}";// date.ToString("yyyy MM dd").Replace(' ', '\n');
            }
            var label = new Label() { Text = headerText, LineBreakMode = LineBreakMode.WordWrap,  Scale = 0.8};
            view.calendarGrid.Add(label, i + 1, 0);
        }
    }

    private static void PrepareGrid(Grid calendarGrid, int milestonesCount)
    {
        calendarGrid.RowDefinitions.Clear();
        calendarGrid.ColumnDefinitions.Clear();
        calendarGrid.Children.Clear();
        for (int i = 0; i < MAX_COLUMNS + 1; i++)
        {
            int width = i == 0 ? 2 : 1;
            calendarGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width, GridUnitType.Star) });
        }

        for (int i = 0; i < milestonesCount + 1; i++)
        {
            var height = i == 0 ? new GridLength(1, GridUnitType.Auto) : new GridLength(1, GridUnitType.Star);
            calendarGrid.RowDefinitions.Add(new RowDefinition() { Height = height });
        }
    }

    private static void AddMilestonesToGrid(Grid calendarGrid, ObservableCollection<Milestone> milestones)
    {
        for (int i = 0; i < milestones.Count(); i++)
        {
            var thumbnail = new MilestoneThumbnailView(milestones[i]);
            calendarGrid.Add(thumbnail, 0, i+1);
        }
    }


    public CalendarView()
    {
        InitializeComponent();
        FillPicker();
        Init();
    }

    private void FillPicker()
    {
        foreach (CalendarUnits unit in Enum.GetValues(typeof(CalendarUnits)))
        {
            calendarUnitDescriptions.Add(new CalendarUnitDescription(unit));
        }
        var unitFromSettings = Preferences.Default.Get(Constants.default_calendar_unit, CalendarUnits.CalendarWeeks.ToString());
        SelectedCalendarUnitDescription = calendarUnitDescriptions.First(_ => _.Unit.ToString() == unitFromSettings);

        timeResolutionPicker.ItemDisplayBinding = new Binding { Path = "Name" };
        timeResolutionPicker.ItemsSource = calendarUnitDescriptions;
        timeResolutionPicker.SelectedIndex = calendarUnitDescriptions.IndexOf(SelectedCalendarUnitDescription);
    }

    private void Init()
    {
        StartingDate = DateTime.Today;
    }

    private void LeftButton_Clicked(object sender, EventArgs e)
    {
        StartingDate.AddDays(-7);
    }

    private void RightButton_Clicked(object sender, EventArgs e)
    {
        StartingDate.AddDays(7);
    }

    private void timeResolutionPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedCalendarUnitDescription = calendarUnitDescriptions[timeResolutionPicker.SelectedIndex];
    }
}