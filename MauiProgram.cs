using System.Text.Json;
using System.Text.Json.Serialization;
using launchapi.Api;
using launchapi.Client;
using launchmaui.VM;
using Microsoft.Extensions.Logging;
using System.Reflection;

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

			var assembly = Assembly.Load("launchapi");
			var converters = assembly
				.GetTypes()
				.Where(t => t.FullName!.EndsWith("JsonConverter"))
				.ToList();

			var launchDetailed = Type.GetType("launchapi.Model.LaunchDetailedJsonConverter, launchapi");
			converters.Add(launchDetailed!);

			foreach (var c in converters)
			{
				var jsonConverter = Activator.CreateInstance(c) as JsonConverter;
				if (jsonConverter is not null)
				{
					options.Converters.Add(jsonConverter);
				}
			}

			return options;
		});

		builder.Services.AddSingleton<JsonSerializerOptionsProvider>(sp =>
		{
			var jsonOptions = sp.GetRequiredService<JsonSerializerOptions>();
			return new JsonSerializerOptionsProvider(jsonOptions);
		});

		builder.Services.AddSingleton<LaunchesApiEvents>();

		builder.Services.AddHttpClient<ILaunchesApi, LaunchesApi>(client =>
		{
			client.BaseAddress = new Uri(ClientUtils.BASE_ADDRESS);
			client.Timeout = TimeSpan.FromMinutes(1);
		});

		builder.Services.AddSingleton<MainVM>();
		builder.Services.AddSingleton<DetailsVM>();

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