using CommunityToolkit.Maui.Storage;

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

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            var jsonStream = _viewModel.GetSerializedMilestones();
            var fileSaverResult = await FileSaver.Default.SaveAsync("new_deadlines.json", jsonStream, CancellationToken.None);
            try
            {
                fileSaverResult.EnsureSuccess();
            }
            catch (Exception ex)
            {
                await DisplayAlert(DeadlineVisualizer.Resources.AppRes.SaveError, ex.Message, "OK");
            }
            finally
            {
                jsonStream?.Dispose();
            }
        }

        private async void OpenButton_Clicked(object sender, EventArgs e)
        {
            var dialogResult = await FilePicker.Default.PickAsync();
            if (dialogResult != null)
            {
                try
                {
                    await _viewModel.LoadFromFileAsync(dialogResult.FullPath);
                }
                catch (Exception ex)
                {
                    await DisplayAlert(DeadlineVisualizer.Resources.AppRes.LoadError, ex.Message, "OK");
                }
            }
        }
    }

}
