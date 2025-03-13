namespace DeadlineVisualizer
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;
        private readonly MilestoneBuffer _milestoneBuffer;
        public MainPage(MainPageViewModel viewModel, MilestoneBuffer milestoneBuffer)
        {
            _viewModel = viewModel;
            _milestoneBuffer = milestoneBuffer;
            BindingContext = _viewModel;
            _viewModel.MilestoneChangeRequested += _viewModel_MilestoneChangeRequested;
            InitializeComponent();
            
        }

        private async void _viewModel_MilestoneChangeRequested(object? sender, Milestone e)
        {
            await Navigation.PushAsync(new DetailsPage(e, _milestoneBuffer), true);
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            _viewModel.OnNavigatedTo();
            base.OnNavigatedTo(args);
        }

    }

}
