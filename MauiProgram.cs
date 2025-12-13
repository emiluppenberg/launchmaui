using System.Text.Json;
using System.Text.Json.Serialization;
using launchmauiclient.Api;
using launchmauiclient.Client;
using launchmauiclient.Model;
using launchmaui.VM;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using launchmaui.Services;

namespace launchmaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder.Services.AddSingleton<JsonSerializerOptions>(sp =>
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = false,
				IncludeFields = true,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			};

			var assembly = typeof(PaginatedPolymorphicLaunchEndpointListJsonConverter).Assembly;
			var converters = assembly
				.GetTypes()
				.Where(t => t.FullName!.EndsWith("JsonConverter"))
				.ToList();

			foreach (var c in converters)
			{
				try
				{
					var jsonConverter = Activator.CreateInstance(c) as JsonConverter;
					if (jsonConverter is not null)
					{
						options.Converters.Add(jsonConverter);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}

			return options;
		});

		builder.Services.AddSingleton<JsonSerializerOptionsProvider>();
		builder.Services.AddSingleton<LaunchesApiEvents>();

		builder.Services.AddHttpClient<ILaunchesApi, LaunchesApi>(client =>
		{
			client.BaseAddress = new Uri(ClientUtils.BASE_ADDRESS);
			client.Timeout = TimeSpan.FromSeconds(10);
		});
		builder.Services.AddHttpClient<ImageValidationService>();

		builder.Services.AddSingleton<MainVM>();

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