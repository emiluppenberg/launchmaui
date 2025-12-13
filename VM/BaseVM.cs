using CommunityToolkit.Mvvm.ComponentModel;

namespace launchmaui.VM;

public partial class BaseVM : ObservableObject
{
  [ObservableProperty]
  string title = "";
}