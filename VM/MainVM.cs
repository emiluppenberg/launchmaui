using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using launchmaui.Utilities;
using launchmaui.VM.Items;
using launchmauiclient.Api;
using launchmauiclient.Model;

namespace launchmaui.VM;

public partial class MainVM : BaseVM
{
  public MainVM(ILaunchesApi launchesApi)
  {
    this._launchesApi = launchesApi;
    this.Title = "All launches";

    _ = NewRequestAsync().ContinueWith(t =>
    {
      if (t.IsFaulted)
      {
        Debug.WriteLine($"MainVM error: {t.Exception?.InnerException?.Message}");
      }
    });
  }

  private CancellationTokenSource? currentCts;
  private readonly ILaunchesApi _launchesApi;

  [ObservableProperty]
  ObservableCollection<LaunchVM> launches = new();

  [ObservableProperty]
  Array pages = new int[1];

  [ObservableProperty]
  int offset;

  [ObservableProperty]
  int page = 1;

  public bool HasNextPage => Page < Pages.Length;
  public bool HasPreviousPage => Page > 1;

  partial void OnPageChanged(int value)
  {
    Offset = (value - 1) * 10;
    OnPropertyChanged(nameof(HasNextPage));
    OnPropertyChanged(nameof(HasPreviousPage));

    Task.Run(async () =>
    {
      await NewRequestAsync();
    }).ContinueWith(t =>
    {
      if (t.IsFaulted)
      {
        Debug.WriteLine($"MainVM error: {t.Exception?.InnerException?.Message}");
      }
    });
  }

  [RelayCommand]
  void NextPage() => Page += 1;

  [RelayCommand]
  void PreviousPage() => Page -= 1;

  private async Task NewRequestAsync()
  {
    currentCts?.Cancel();
    currentCts = new CancellationTokenSource();
    var ct = currentCts.Token;

    await Task.Run(async () =>
    {
      try
      {
        Launches.Clear();

        var response = await _launchesApi.LaunchesUpcomingListAsync(offset: Offset, cancellationToken: ct);
        if (response is null || !response.IsOk) return;
        var list = response.Ok()!;

        var pagesCount = (int)Math.Ceiling(list.Count / 10.0);
        Pages = Enumerable.Range(1, pagesCount).ToArray();
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(HasPreviousPage));

        var vms = new List<LaunchVM>();
        foreach (var r in list.Results)
        {
          ct.ThrowIfCancellationRequested();

          if (r.LaunchNormal is null)
          {
            continue;
          }

          var vm = new LaunchVM(r.LaunchNormal.Id, r.LaunchNormal.Name, r.LaunchNormal.WindowStart, r.LaunchNormal.WindowEnd, r.LaunchNormal.Image?.ImageUrl, LaunchTypes.Basic);
          vms.Add(vm);
        }

        await MainThread.InvokeOnMainThreadAsync(() =>
              {
                Launches.Clear();
                vms.ForEach(vm => Launches.Add(vm));
              });
      }
      catch (OperationCanceledException ex)
      {
        Debug.WriteLine($"Request canceled: {ex.Message}");
      }
      catch (Exception ex)
      {
        Debug.WriteLine("MainVM error: ", ex.Message);
      }
    });
  }
}