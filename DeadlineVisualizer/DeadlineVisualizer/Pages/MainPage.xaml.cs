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
            if (string.IsNullOrEmpty(_viewModel.CurrentFileFullPath))
            {
                SaveAsButton_Clicked(sender, e);
            }
            else
            {
                try
                {
                    using (var jsonStream = _viewModel.GetSerializedMilestones())
                    using (var fileStream = new FileStream(_viewModel.CurrentFileFullPath, FileMode.Create, FileAccess.Write))
                    {
                        await jsonStream.CopyToAsync(fileStream);
                    };
                    _viewModel.IsDirty = false;
                }
                catch (Exception ex)
                {
                    await DisplayAlert(DeadlineVisualizer.Resources.AppRes.SaveError, ex.Message, "OK");
                }
            }
        }

        private async void SaveAsButton_Clicked(object sender, EventArgs e)
        {
            var jsonStream = _viewModel.GetSerializedMilestones();
            var fileSaverResult = await FileSaver.Default.SaveAsync("new_deadlines.json", jsonStream, CancellationToken.None);
            try
            {
                fileSaverResult.EnsureSuccess();
                _viewModel.CurrentFileFullPath = fileSaverResult.FilePath;
                _viewModel.IsDirty = false;
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
            if(!(await AskToProceed()))
            {
                return;
            }
            var dialogResult = await FilePicker.Default.PickAsync();
            if (dialogResult != null)
            {
                try
                {
                    await _viewModel.LoadFromFileAsync(dialogResult.FullPath);
                    _viewModel.CurrentFileFullPath = dialogResult.FullPath;
                }
                catch (Exception ex)
                {
                    await DisplayAlert(DeadlineVisualizer.Resources.AppRes.LoadError, ex.Message, "OK");
                }
            }
        }

        private async Task<bool> AskToProceed()
        {
            if (_viewModel.IsDirty)
            {
                var answer = await DisplayAlert(DeadlineVisualizer.Resources.AppRes.CurrentFileHasBeenModified,
                    DeadlineVisualizer.Resources.AppRes.ProceedWithoutSaving,
                    DeadlineVisualizer.Resources.AppRes.Yes,
                    DeadlineVisualizer.Resources.AppRes.No);
                return answer;
            }
            return true;
        }

        private async void NewButton_Clicked(object sender, EventArgs e)
        {
            if (!(await AskToProceed()))
            {
                return;
            }
            _viewModel.Clear();
        }
    }
}
