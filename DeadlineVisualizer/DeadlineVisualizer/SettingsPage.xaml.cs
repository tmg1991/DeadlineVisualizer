namespace DeadlineVisualizer;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsPageViewModel _viewModel;
	public SettingsPage(SettingsPageViewModel viewModel)
	{
		_viewModel = viewModel;
		BindingContext = _viewModel;
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
		_viewModel.OnAppearing();
        base.OnAppearing();
    }
}