using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using launchmaui.Services;
using launchmaui.Utilities;
using launchmaui.VM.Items;
using launchmauiclient.Api;
using launchmauiclient.Model;

namespace launchmaui.VM;

public partial class MainVM : BaseVM
{
  public MainVM(ILaunchesApi launchesApi, ImageValidationService imageValidationService)
  {
    this._launchesApi = launchesApi;
    this.imageValidationService = imageValidationService;
    this.Title = "All launches";
  }

  private readonly ILaunchesApi _launchesApi;
  private readonly ImageValidationService imageValidationService;

  [ObservableProperty]
  ObservableCollection<LaunchEndpointVM> endpoints = new();

  [ObservableProperty]
  ObservableCollection<LaunchBasic> launchBasicCache = new();

  [ObservableProperty]
  ObservableCollection<LaunchNormal> launchNormalCache = new();

  [ObservableProperty]
  ObservableCollection<LaunchDetailed> launchDetailedCache = new();

  [RelayCommand]
  async Task GetLaunches()
  {
    try
    {
      var launches = await _launchesApi.LaunchesListAsync();
      if (!launches.IsOk) return;
      var list = launches.Ok()!;

      LaunchBasicCache.Clear();
      LaunchNormalCache.Clear();
      LaunchDetailedCache.Clear();
      Endpoints.Clear();

      var stopwatch = Stopwatch.StartNew();
      foreach (var r in list.Results)
      {
        if (r.LaunchBasic is not null)
        {
          var imageUrl = await this.imageValidationService.ValidateImageUrl(r.LaunchBasic.Image?.ImageUrl);
          var vm = new LaunchEndpointVM(r.LaunchBasic.Id, r.LaunchBasic.Name, imageUrl, LaunchTypes.Basic);

          Endpoints.Add(vm);
          LaunchBasicCache.Add(r.LaunchBasic);
        }
        if (r.LaunchNormal is not null)
        {
          var imageUrl = await this.imageValidationService.ValidateImageUrl(r.LaunchNormal.Image?.ImageUrl);
          var vm = new LaunchEndpointVM(r.LaunchNormal.Id, r.LaunchNormal.Name, imageUrl, LaunchTypes.Basic);

          Endpoints.Add(vm);
          LaunchNormalCache.Add(r.LaunchNormal);
        }
        if (r.LaunchDetailed is not null)
        {
          var imageUrl = await this.imageValidationService.ValidateImageUrl(r.LaunchDetailed.Image?.ImageUrl);
          var vm = new LaunchEndpointVM(r.LaunchDetailed.Id, r.LaunchDetailed.Name, imageUrl, LaunchTypes.Basic);

          Endpoints.Add(vm);
          LaunchDetailedCache.Add(r.LaunchDetailed);
        }
      }
      stopwatch.Stop();

      Debug.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}");
    }
    catch (Exception ex)
    {
      Debug.WriteLine(ex.Message);
    }
  }
}