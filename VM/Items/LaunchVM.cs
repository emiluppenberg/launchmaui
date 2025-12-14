using CommunityToolkit.Mvvm.ComponentModel;
using launchmaui.Utilities;

namespace launchmaui.VM.Items;

public partial class LaunchVM(Guid id, string? name, DateTime? windowStart, DateTime? windowEnd, string? url, LaunchTypes launchType) : BaseVM
{
  [ObservableProperty]
  string id = id.ToString();

  [ObservableProperty]
  string displayName = name is not null ? name : id.ToString();

  [ObservableProperty]
  DateTime? windowStart = windowStart;

  [ObservableProperty]
  DateTime? windowEnd = windowEnd;

  [ObservableProperty]
  string imageUrl = url is not null ? url : "";

  [ObservableProperty]
  LaunchTypes launchType = launchType;
}