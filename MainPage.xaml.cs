using launchclient.Api;

namespace launchmaui;

public partial class MainPage : ContentPage
{
	private readonly ILaunchesApi launchesApi;

	public MainPage(ILaunchesApi launchesApi)
	{
		InitializeComponent();
		this.launchesApi = launchesApi;
	}

	public async void GetLaunches(object sender, EventArgs e)
	{
		var launches = await launchesApi.LaunchesListAsync();
	}
}
