using CommunityToolkit.Mvvm.ComponentModel;

namespace launchmaui.VM.Items;

public partial class LaunchVM(Guid id, string? missionName, DateTime? net, string? thumbnailUrl, string lspName, string? lspAbbrev) : BaseVM
{
  [ObservableProperty]
  string id = id.ToString();

  [ObservableProperty]
  string? missionName = !string.IsNullOrEmpty(missionName) || !string.IsNullOrWhiteSpace(missionName) ? missionName : "no name provided";

  [ObservableProperty]
  DateTime? net = net;

  [ObservableProperty]
  string thumbnailUrl = !string.IsNullOrEmpty(thumbnailUrl) || !string.IsNullOrWhiteSpace(thumbnailUrl) ? thumbnailUrl : "fallback.jpg";

  [ObservableProperty]
  string? lspName = lspName.Length > 25 && !string.IsNullOrEmpty(lspAbbrev) && !string.IsNullOrWhiteSpace(lspAbbrev) ? lspAbbrev : lspName;
}