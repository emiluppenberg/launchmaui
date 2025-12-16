using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace launchmaui.VM;

public partial class DetailsVM : BaseVM
{
  [ObservableProperty]
  DetailsVM launchDetails = null!;

  [RelayCommand]
  async Task GoToMain()
  {
    await Shell.Current.GoToAsync($"///{nameof(MainPage)}");
  }
}