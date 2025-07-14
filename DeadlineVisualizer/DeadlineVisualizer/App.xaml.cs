#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using Microsoft.Maui.Platform; // Needed for PlatformView
#endif


namespace DeadlineVisualizer
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
            InitPreferences();
            MainPage = new AppShell();
        }

        private void InitPreferences()
        {
            SetDefaultDistance(Constants.warning_level_1_distance_key, Constants.warning_level_1_distance);
            SetDefaultDistance(Constants.warning_level_2_distance_key, Constants.warning_level_2_distance);
            SetDefaultDistance(Constants.warning_level_3_distance_key, Constants.warning_level_3_distance);
            if(!Preferences.Default.ContainsKey(Constants.default_calendar_unit))
            {
                Preferences.Default.Set(Constants.default_calendar_unit, CalendarUnits.CalendarWeeks.ToString());
            }

            void SetDefaultDistance(string key, int days){
                if (!Preferences.Default.ContainsKey(key))
                {
                    Preferences.Default.Set(key, days);
                }
            }

        }

        protected override Microsoft.Maui.Controls.Window CreateWindow(IActivationState activationState)
        {
            var mauiWindow = new Microsoft.Maui.Controls.Window(MainPage);

#if WINDOWS
        mauiWindow.HandlerChanged += (s, e) =>
        {
        if(mauiWindow.Handler == null)
            {
                return;
            }
            var nativeWindow = mauiWindow.Handler.PlatformView as Microsoft.UI.Xaml.Window;

            var hwnd = WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.Closing += async (s, e) =>
            {
                e.Cancel = true;
                var result = await ShowCloseConfirmationAsync(mauiWindow.Page);

                if (result)
                {
                    //appWindow.Destroy(); // Now we close for real
                    nativeWindow.Close();
                }
            };
        };
#endif

            return mauiWindow;
        }


#if WINDOWS
    private async Task<bool> ShowCloseConfirmationAsync(Page page)
    {
        return await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            return await page.DisplayAlert(
                DeadlineVisualizer.Resources.AppRes.AppIsAboutToClose,
                DeadlineVisualizer.Resources.AppRes.RemindToCheck,
                DeadlineVisualizer.Resources.AppRes.Yes,
                DeadlineVisualizer.Resources.AppRes.No
            );
        });
    }
#endif
    }
}
