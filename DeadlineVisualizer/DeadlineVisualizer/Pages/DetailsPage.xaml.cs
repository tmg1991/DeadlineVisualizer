namespace DeadlineVisualizer;

public partial class DetailsPage : ContentPage
{
	private readonly Milestone _milestone;
	private readonly MilestoneBuffer _milestoneBuffer;
	int counter = 0;
   
    public Milestone MilestoneClone { get; private set; }

	private string _errorMessage;

	public string ErrorMessage
	{
		get { return _errorMessage; }
		set { _errorMessage = value; OnPropertyChanged(); }
	}


	public List<Color> AvailableColors { get; private set; } = new List<Color>() { Colors.Lavender, Colors.CornflowerBlue, Colors.DarkSalmon, Colors.DarkSeaGreen, Colors.Gray, Colors.Plum, Colors.Crimson };

    public DetailsPage(Milestone milestone, MilestoneBuffer milestoneBuffer)
	{
		_milestone = milestone;
		_milestoneBuffer = milestoneBuffer;
		MilestoneClone = new Milestone()
		{
			Title = milestone.Title,
			Deadline = milestone.Deadline,
			Color = milestone.Color,
			Notes = milestone.Notes,
			WarningLevel1 = milestone.WarningLevel1,
			WarningLevel2 = milestone.WarningLevel2,
			WarningLevel3 = milestone.WarningLevel3
		};
		BindingContext = this;
		InitializeComponent();
	}

    private void ColorButton_Clicked(object sender, EventArgs e)
    {
		if(counter == AvailableColors.Count() - 1 ) { counter = -1; }
		MilestoneClone.Color = AvailableColors[++counter];
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
		if (string.IsNullOrEmpty(MilestoneClone.Title))
		{
			ErrorMessage = DeadlineVisualizer.Resources.AppRes.TitleShouldNotBeEmpty;
            return;
		}
		if(MilestoneClone.Deadline < DateTime.Today)
		{
			ErrorMessage = DeadlineVisualizer.Resources.AppRes.DeadlineShouldNotBeInThePast;
            return;
		}

		if (!(AreDistancesValid(MilestoneClone))){
            ErrorMessage = DeadlineVisualizer.Resources.AppRes.SettingsErrorMessage;
            return;
		}

		_milestone.Title = MilestoneClone.Title;
		_milestone.Deadline = MilestoneClone.Deadline;
		_milestone.Color = MilestoneClone.Color;
		_milestone.Notes = MilestoneClone.Notes;
		_milestone.WarningLevel1 = MilestoneClone.WarningLevel1;
		_milestone.WarningLevel2 = MilestoneClone.WarningLevel2;
		_milestone.WarningLevel3 = MilestoneClone.WarningLevel3;

		_milestoneBuffer.Enqueue( _milestone );
		Navigation.RemovePage(this);
    }

    private bool AreDistancesValid(Milestone milestone) => milestone.WarningLevel3 > 0 &&
            milestone.WarningLevel2 > milestone.WarningLevel3 &&
            milestone.WarningLevel1 > milestone.WarningLevel2;

}