using CommunityToolkit.Maui.Storage;

namespace DeadlineVisualizer
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;
        private readonly MilestoneBuffer _milestoneBuffer;
        private FileSystemWatcher _fileSystemWatcher;
        private bool _isReloadNeeded;
        private DateTime _lastChangedTime = DateTime.MinValue;
        private readonly TimeSpan _debounceTime = TimeSpan.FromSeconds(1);
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
                    StopWatchingFile();
                    using (var jsonStream = _viewModel.GetSerializedMilestones())
                    using (var fileStream = new FileStream(_viewModel.CurrentFileFullPath, FileMode.Create, FileAccess.Write))
                    {
                        await jsonStream.CopyToAsync(fileStream);
                    };
                    _viewModel.IsDirty = false;
                    StartWatchingFile();
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
                StopWatchingFile();
                _viewModel.CurrentFileFullPath = fileSaverResult.FilePath;
                _viewModel.IsDirty = false;
                StartWatchingFile();
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
            if(!await AskToProceed())
            {
                return;
            }
            var dialogResult = await FilePicker.Default.PickAsync();
            if (dialogResult != null)
            {
                await LoadFile(dialogResult.FullPath);
            }
        }

        private async Task LoadFile(string fullPath)
        {
            try
            {
                StopWatchingFile();
                await _viewModel.LoadFromFileAsync(fullPath);
                _viewModel.CurrentFileFullPath = fullPath;
                StartWatchingFile();
            }
            catch (Exception ex)
            {
                await DisplayAlert(DeadlineVisualizer.Resources.AppRes.LoadError, ex.Message, "OK");
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

        private async Task<bool> AskForReload()
        {
            var answer = await DisplayAlert(DeadlineVisualizer.Resources.AppRes.CurrentFileHasBeenModified,
                    DeadlineVisualizer.Resources.AppRes.WouldYouLikeToReloadFile,
                    DeadlineVisualizer.Resources.AppRes.Yes,
                    DeadlineVisualizer.Resources.AppRes.No);
            return answer;
        }

        private async void NewButton_Clicked(object sender, EventArgs e)
        {
            if (!await AskToProceed())
            {
                return;
            }
            StopWatchingFile();
            _viewModel.Clear();
        }

        private void StartWatchingFile()
        {
            var path = Path.GetDirectoryName(_viewModel.CurrentFileFullPath);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            _fileSystemWatcher = new FileSystemWatcher(path)
            {
                Filter = Path.GetFileName(_viewModel.CurrentFileFullPath),
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };
            _fileSystemWatcher.Changed += OnWatchedFileChanged;
        }

        private void StopWatchingFile()
        {
            if (_fileSystemWatcher == null)
            {
                return;
            }
            _fileSystemWatcher.Changed -= OnWatchedFileChanged;
            _fileSystemWatcher.Dispose();
        }

        private void OnWatchedFileChanged(object sender, FileSystemEventArgs e)
        {
            var now = DateTime.Now;
            if (now - _lastChangedTime < _debounceTime)
            {
                return;
            }

            _lastChangedTime = now;
            SafeAskForReload();
        }

        private void SafeAskForReload()
        {
            if (_isReloadNeeded == true)
            {
                return;
            }
            _isReloadNeeded = true;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (await AskForReload())
                {
                    await LoadFile(_viewModel.CurrentFileFullPath);
                    _isReloadNeeded = false;
                }
            });
        }
    }
}
