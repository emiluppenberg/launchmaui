using CommunityToolkit.Mvvm.ComponentModel;
using launchmaui.Utilities;

namespace launchmaui.VM.Items;

public partial class LaunchEndpointVM(Guid id, string? name, string url, LaunchTypes launchType) : BaseVM
{
  [ObservableProperty]
  string id = id.ToString();

  [ObservableProperty]
  string displayName = name is not null ? name : id.ToString();

  [ObservableProperty]
  string imageUrl = url;

  [ObservableProperty]
  LaunchTypes launchType = launchType;
}