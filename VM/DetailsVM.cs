using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using launchmaui.VM.Items;

namespace launchmaui.VM;

[QueryProperty(nameof(Details), "Details")]
public partial class DetailsVM : BaseVM
{
  [ObservableProperty]
  LaunchDetailsVM details = null!;

  [RelayCommand]
  async Task GoToMain()
  {
    await Shell.Current.GoToAsync($"///{nameof(MainPage)}");
  }

  [RelayCommand]
  async Task OpenUrl(string url)
  {
    if (!string.IsNullOrEmpty(url))
    {
      try
      {
        await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error browser: {ex.Message}");
      }
    }
  }
}