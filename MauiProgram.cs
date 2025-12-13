using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace launchmaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder.Services.AddSingleton<JsonSerializerOptions>(new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = false
		});
		builder.Services.AddSingleton<JsonSerializerOptionsProvider>();
		builder.Services.AddSingleton<LaunchesApiEvents>();
		builder.Services.AddHttpClient<ILaunchesApi, LaunchesApi>(c => { c.BaseAddress = new Uri(ClientUtils.BASE_ADDRESS); });

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}