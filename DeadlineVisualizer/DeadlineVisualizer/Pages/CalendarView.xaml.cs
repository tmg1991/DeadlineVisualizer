using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DeadlineVisualizer;

public partial class CalendarView : ContentView
{
    private const int MAX_COLUMNS = 15;
    public static readonly BindableProperty MilestonesProperty =
  BindableProperty.Create(nameof(Milestone), typeof(ObservableCollection<Milestone>), typeof(CalendarView), default(ObservableCollection<Milestone>), propertyChanged: OnMilestonesChanged);

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
        
    }

    private static void PrepareGrid(Grid calendarGrid, int milestonesCount)
    {
        calendarGrid.RowDefinitions.Clear();
        calendarGrid.ColumnDefinitions.Clear();
        calendarGrid.Children.Clear();
        for (int i = 0; i < MAX_COLUMNS + 1; i++)
        {
            calendarGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        }

        for (int i = 0; i < milestonesCount + 1; i++)
        {
            calendarGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        }
    }

    private static void AddMilestonesToGrid(Grid calendarGrid, ObservableCollection<Milestone> milestones)
    {
        for (int i = 0; i < milestones.Count(); i++)
        {
            var thumbnail = new MilestoneThumbnailView(milestones[i]);
            calendarGrid.Add(thumbnail, 0, i);
        }
    }


    public CalendarView()
    {
        InitializeComponent();
        FillPicker();
    }

    private void FillPicker()
    {
        List<CalendarUnitDescription> calendarUnitDescriptions = new();
        foreach (CalendarUnits unit in Enum.GetValues(typeof(CalendarUnits)))
        {
            calendarUnitDescriptions.Add(new CalendarUnitDescription(unit));
        }
        var unitFromSettings = Preferences.Default.Get(Constants.default_calendar_unit, CalendarUnits.CalendarWeeks.ToString());
        var selectedCalendarUnitDescription = calendarUnitDescriptions.First(_ => _.Unit.ToString() == unitFromSettings);

        timeResolutionPicker.ItemDisplayBinding = new Binding { Path = "Name" };
        timeResolutionPicker.ItemsSource = calendarUnitDescriptions;
        timeResolutionPicker.SelectedIndex = calendarUnitDescriptions.IndexOf(selectedCalendarUnitDescription);
    }
}