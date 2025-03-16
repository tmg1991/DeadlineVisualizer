namespace DeadlineVisualizer;

public partial class MilestoneThumbnailView : ContentView
{
	private readonly MilestoneThumbnailViewModel _viewModel;
    private readonly MilestoneBuffer _milestoneBuffer;

    

    public MilestoneThumbnailView(Milestone milestone)
	{
        var context = Application.Current.Handler.MauiContext;
        _milestoneBuffer = context.Services.GetService<MilestoneBuffer>();
        _viewModel = new MilestoneThumbnailViewModel(milestone);
		BindingContext = _viewModel;
        _viewModel.MilestoneChangeRequested += ViewModel_MilestoneChangeRequested;
		InitializeComponent();
	}

    private async void ViewModel_MilestoneChangeRequested(object? sender, Milestone e)
    {
        await Navigation.PushAsync(new DetailsPage(e, _milestoneBuffer), true);
    }
}