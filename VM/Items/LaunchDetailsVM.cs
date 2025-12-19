using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using launchapi.Model;

namespace launchmaui.VM.Items;

public partial class LaunchDetailsVM : BaseVM
{
	[ObservableProperty]
	string? thumbnailUrl; // ImageModel -> ThumbnailUrl

	[ObservableProperty]
	string? missionName; // Mission -> Name

	[ObservableProperty]
	string? missionDescription; // Mission -> Description

	[ObservableProperty]
	string? rocketName; // Rocket -> Configuration -> Name

	[ObservableProperty]
	string? rocketDescription; // Rocket -> Configuration -> Description

	[ObservableProperty]
	string? landingName; // Rocket -> LauncherStage -> Landing -> LandingLocation -> Name

	[ObservableProperty]
	string? landingThumbnailUrl; // Rocket -> LauncherStage -> Landing -> LandingLocation -> ImageLandingLocation -> ThumbnailUrl

	[ObservableProperty]
	string? lspName; // LaunchServiceProvider -> Name

	[ObservableProperty]
	string? lspThumbnailUrl; // LaunchServiceProvider -> SocialLogo -> ThumbnailUrl

	[ObservableProperty]
	string? padName; // Pad -> Name

	[ObservableProperty]
	string? padMapImageUrl; // Pad -> MapImage

	[ObservableProperty]
	string? padLocationName; // Pad -> Location -> Name

	[ObservableProperty]
	string? url; // Url

	[ObservableProperty]
	DateTime? net; // Net

	[ObservableProperty]
	DateTime? windowStart; // WindowStart

	[ObservableProperty]
	DateTime? windowEnd; // WindowEnd

	[ObservableProperty]
	string? statusName; // Status -> Name

	[ObservableProperty]
	string? statusDescription; // Status -> Description

	[ObservableProperty]
	string? weatherConcerns; // WeatherConcerns

	public static LaunchDetailsVM CreateFromModel(LaunchDetailed model)
	{
		return new LaunchDetailsVM
		{
			LandingName = model.Rocket?.LauncherStage?.FirstOrDefault()?.Landing.LandingLocation?.Name,
			LandingThumbnailUrl = model.Rocket?.LauncherStage?.FirstOrDefault()?.Landing.LandingLocation?.Image?.ThumbnailUrl,
			LspName = model.LaunchServiceProvider?.Name,
			LspThumbnailUrl = model.LaunchServiceProvider?.SocialLogo?.ThumbnailUrl,
			MissionDescription = model.Mission?.Description,
			MissionName = model.Mission?.Name,
			Net = model.Net,
			PadLocationName = model.Pad?.Location?.Name,
			PadMapImageUrl = model.Pad?.MapImage,
			PadName = model.Pad?.Name,
			RocketDescription = model.Rocket?.VarConfiguration?.Description,
			RocketName = model.Rocket?.VarConfiguration?.Name,
			StatusDescription = model.Status?.Description,
			StatusName = model.Status?.Name,
			ThumbnailUrl = model.Image?.ThumbnailUrl,
			Url = model.Url,
			WeatherConcerns = model.WeatherConcerns,
			WindowEnd = model.WindowEnd,
			WindowStart = model.WindowStart,
			Title = $"{model.LaunchServiceProvider?.Name} | {model.Rocket?.VarConfiguration?.Name} | {model.Mission?.Name}"
		};
	}

	[DebuggerStepThrough]
	public static LaunchDetailsVM? CreateFromJson(string rawContent)
	{
		var model = new Dictionary<string, Dictionary<string, object?>>();

		var modelCurrentParents = new List<string>() { "Model" };
		var modelObjects = new List<KeyValuePair<string, string[][]>>
		{
			new("Model",[
				["Model"],
				["Url", "Net", "WindowStart", "WindowEnd", "WeatherConcerns"]]),
			new("Image", [
				["Model", "Image"],
				["ThumbnailUrl"]]),
			new("Mission", [
				["Model", "Mission"],
				["Name", "Description"]]),
			new("Configuration", [
				["Model", "Rocket", "Configuration"],
				["Name", "Description"]]),
			new("LaunchServiceProvider",[
				["Model", "LaunchServiceProvider"],
				["Name"]]),
			new("SocialLogo", [
				["Model", "LaunchServiceProvider", "SocialLogo"],
				["ThumbnailUrl"]]),
			new("LandingLocation", [
				["Model", "Rocket", "LauncherStage", "Landing", "LandingLocation"],
				["Name"]]),
			new("Image", [
				["Model", "Rocket", "LauncherStage", "Landing", "LandingLocation", "Image"],
				["ThumbnailUrl"]]),
			new("Pad", [
				["Model", "Pad"],
				["Name", "MapImage"]]),
			new("Location", [
				["Model", "Pad", "Location"],
				["Name"]]),
			new("Status", [
				["Model", "Status"],
				["Name", "Description"]])
		};

		var arrayCurrentParents = new List<KeyValuePair<string, List<string>>>();
		var arrayObjects = new List<KeyValuePair<string, string[][]>>
		{
			new("LauncherStage",[
				["Model", "Rocket", "LauncherStage"],
				["LauncherFlightNumber"]
			]),
			new("LauncherStage",[
				["Model", "Rocket", "LauncherStage", "Launcher"],
				["FirstLaunchDate", "LastLaunchDate"]
			]),
			new("LauncherStage",[
				["Model", "Rocket", "LauncherStage", "Landing" ,"LandingLocation"],
				["Name"]
			]),
			new("LauncherStage",[
				["Model", "Rocket", "LauncherStage", "Landing" ,"LandingLocation", "Location"],
				["MapImage"]
			]),
			new("Families", [
				["Model", "Rocket", "Configuration", "Families"],
				["Name"]
			]),
			new("Manufacturer", [
				["Model","Rocket", "Configuration", "Families", "Manufacturer"],
				["Name"]
			]),
			new("InfoUrls",[
				["Model", "InfoUrls"],
				["Url"]
			]),
			new("Timeline",[
				["Model", "Timeline"],
				["RelativeTime"]
			])
		};

		var isArray = false;
		var bytes = Encoding.UTF8.GetBytes(rawContent);
		var reader = new Utf8JsonReader(bytes);
		try
		{
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject && !isArray)
				{
					modelCurrentParents.RemoveAt(modelCurrentParents.Count() - 1);
					continue;
				}

				if (reader.TokenType == JsonTokenType.EndObject && isArray)
				{
					var last = arrayCurrentParents.LastOrDefault();

					if (!last.Value.Last().Equals(last.Key))
					{
						last.Value.RemoveAt(last.Value.Count() - 1);
					}

					continue;
				}

				if (reader.TokenType == JsonTokenType.EndArray)
				{
					if (arrayCurrentParents.Count() > 0)
					{
						arrayCurrentParents.RemoveAt(arrayCurrentParents.Count() - 1);
						isArray = false;
					}

					continue;
				}

				if (reader.TokenType == JsonTokenType.PropertyName)
				{
					string? currentProperty = Regex.Replace(reader.GetString()!, @"(?:^|_)([a-z])",
						match => match.Groups[1].Value.ToUpper());

					reader.Read();

					if (reader.TokenType == JsonTokenType.StartArray)
					{
						var lineage = new List<string>(modelCurrentParents) { currentProperty };
						var match = arrayObjects.Where(a => a.Key == currentProperty && a.Value[0].SequenceEqual(lineage)).ToList();

						if (match.Count() > 0)
						{
							arrayCurrentParents.Add(new(currentProperty, lineage));
							isArray = true;
							continue;
						}

						if (match.Count() == 0)
						{
							reader.Skip();
							continue;
						}
					}

					if (reader.TokenType == JsonTokenType.StartObject && !isArray)
					{
						modelCurrentParents.Add(currentProperty);
						continue;
					}

					if (reader.TokenType == JsonTokenType.StartObject && isArray)
					{
						arrayCurrentParents.LastOrDefault().Value.Add(currentProperty);
						continue;
					}

					var (isEqual, matchingIndex, matches) = isArray is false ?
					 IsModelLineage(modelCurrentParents, modelObjects, currentProperty) :
					 IsArrayLineage(arrayCurrentParents, arrayObjects, currentProperty);

					if (!isEqual || matchingIndex is null || matches is null)
					{
						continue;
					}

					object? value = null;

					if (reader.TokenType == JsonTokenType.String)
					{
						value = reader.GetString();
					}

					if (reader.TokenType == JsonTokenType.Number)
					{
						if (reader.TryGetInt64(out long longVal))
						{
							value = longVal;
						}
						if (reader.TryGetDouble(out double doubleVal))
						{
							value = doubleVal;
						}
					}

					if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
					{
						value = reader.GetBoolean();
					}

					var currentObject = isArray is false ? modelCurrentParents.Last() : arrayCurrentParents.Last().Key;

					if (!isArray && modelCurrentParents.Count() > 1)
					{
						var parentIndex = matches[(int)matchingIndex].Value[0].Length - 2;
						currentObject = matches[(int)matchingIndex].Value[0][parentIndex] + currentObject;
					}
					if (isArray)
					{
						var nestIndex = arrayCurrentParents.Count() - 1;
						var currentLevel = arrayCurrentParents[nestIndex];
						var currentArray = nestIndex > 1 ?
							arrayCurrentParents[nestIndex - 1].Key + currentLevel.Key :
							currentLevel.Value[currentLevel.Value.IndexOf(currentLevel.Key) - 1] + currentLevel.Key;

						currentObject = currentArray;

						if (nestIndex > 1)
						{
							arrayCurrentParents.ForEach(p => Debug.Write($"{p.Key} "));
							Debug.WriteLine("");
						}
					}

					if (!isArray)
					{
						if (!model.ContainsKey(currentObject))
						{
							model.Add(currentObject, new Dictionary<string, object?> { [currentProperty] = value });
						}
						else
						{
							model[currentObject].Add(currentProperty, value);
						}
					}
					if (isArray)
					{
						if (!model.ContainsKey(currentObject))
						{
							var list = new List<object?>() { value };
							model.Add(currentObject, new Dictionary<string, object?> { [currentProperty] = list });
						}
						else
						{
							var list = new List<object?>() { value };

							if (!model[currentObject].ContainsKey(currentProperty))
							{
								model[currentObject].Add(currentProperty, list);
							}
							else
							{
								list = model[currentObject][currentProperty] as List<object?>;
								list!.Add(value);
							}
						}
					}
				}
			}

			foreach (var k in model.Keys)
			{
				foreach (var kvp in model[k])
				{
					Debug.WriteLine($"{k}.{kvp.Key} - {kvp.Value}");
				}
			}

			return new LaunchDetailsVM
			{
				LandingName = model.TryGetValue("LandingLandingLocation", out var landingLocation)
					&& landingLocation.TryGetValue("Name", out var landingName)
					? (string?)landingName
					: null,

				LandingThumbnailUrl = model.TryGetValue("LandingLocationImage", out var imageLandingLocation)
					&& imageLandingLocation.TryGetValue("ThumbnailUrl", out var landingThumbnailUrl)
					? (string?)landingThumbnailUrl
					: null,

				LspName = model.TryGetValue("LaunchServiceProvider", out var lsp)
					&& lsp.TryGetValue("Name", out var lspName)
					? (string?)lspName
					: null,

				LspThumbnailUrl = model.TryGetValue("LaunchServiceProviderSocialLogo", out var socialLogo)
					&& socialLogo.TryGetValue("ThumbnailUrl", out var lspThumbnailUrl)
					? (string?)lspThumbnailUrl
					: null,

				MissionDescription = model.TryGetValue("ModelMission", out var mission)
					&& mission.TryGetValue("Description", out var missionDescription)
					? (string?)missionDescription
					: null,

				MissionName = mission is not null && mission.TryGetValue("Name", out var missionName)
					? (string?)missionName
					: null,

				PadLocationName = model.TryGetValue("PadLocation", out var location)
					&& location.TryGetValue("Name", out var padLocationName)
					? (string?)padLocationName
					: null,

				PadMapImageUrl = model.TryGetValue("ModelPad", out var pad)
					&& pad.TryGetValue("MapImage", out var padMapImage)
					? (string?)padMapImage
					: null,

				PadName = pad is not null && pad.TryGetValue("Name", out var padName)
					? (string?)padName
					: null,

				RocketDescription = model.TryGetValue("RocketConfiguration", out var configuration)
					&& configuration.TryGetValue("Description", out var rocketDescription)
					? (string?)rocketDescription
					: null,

				RocketName = configuration is not null && configuration.TryGetValue("Name", out var rocketName)
					? (string?)rocketName
					: null,

				StatusDescription = model.TryGetValue("ModelStatus", out var status)
					&& status.TryGetValue("Description", out var statusDescription)
					? (string?)statusDescription
					: null,

				StatusName = status is not null && status.TryGetValue("Name", out var statusName)
					? (string?)statusName
					: null,

				ThumbnailUrl = model.TryGetValue("ModelImage", out var imageModel)
					&& imageModel.TryGetValue("ThumbnailUrl", out var thumbnailUrl)
					? (string?)thumbnailUrl
					: null,

				Url = model["Model"].TryGetValue("Url", out var url)
					? (string?)url
					: null,

				WeatherConcerns = model["Model"].TryGetValue("WeatherConcerns", out var weather)
					? (string?)weather
					: null,

				Net = model["Model"].TryGetValue("Net", out var netObj)
					&& netObj is string netStr
					&& DateTime.TryParse(netStr, out var netDate)
					? netDate
					: null,

				WindowEnd = model["Model"].TryGetValue("WindowEnd", out var windowEndObj)
					&& windowEndObj is string windowEndStr
					&& DateTime.TryParse(windowEndStr, out var windowEndDate)
					? windowEndDate
					: null,

				WindowStart = model["Model"].TryGetValue("WindowStart", out var windowStartObj)
					&& windowStartObj is string windowStartStr
					&& DateTime.TryParse(windowStartStr, out var windowStartDate)
					? windowStartDate
					: null,

				Title = $"{(model.TryGetValue("LaunchServiceProvider", out var titleLsp)
							&& titleLsp.TryGetValue("Name", out var titleLspName)
							? (string?)titleLspName
							: "")} | {(model.TryGetValue("RocketConfiguration", out var titleConfiguration)
							&& titleConfiguration.TryGetValue("Name", out var titleRocketName)
							? (string?)titleRocketName
							: "")} | {(model.TryGetValue("Mission", out var titleMission)
							&& titleMission.TryGetValue("Name", out var titleMissionName)
							? (string?)titleMissionName
							: "")}"
			};
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"// BASE - {ex.GetBaseException().Message} // INNER - {ex.InnerException?.Message} // SOURCE - {ex.Source} // STACKTRACE - {ex.StackTrace} // TARGETSITE - {ex.TargetSite}");
			return null;
		}
	}

	private static (bool, int?, List<KeyValuePair<string, string[][]>>?) IsModelLineage(
		List<string> modelCurrentParents,
		List<KeyValuePair<string, string[][]>> modelObjects,
		string currentProperty)
	{
		var currentObject = modelCurrentParents.Last();
		var matchingObjects = modelObjects
			.Where(o => o.Key == currentObject)
			.ToList();

		if (matchingObjects.Any(o => o.Value[1].Contains(currentProperty)))
		{
			return CompareLineage(modelCurrentParents, matchingObjects);
		}

		return (false, null, null);
	}

	private static (bool, int?, List<KeyValuePair<string, string[][]>>?) IsArrayLineage(
		List<KeyValuePair<string, List<string>>> arrayCurrentParents,
		List<KeyValuePair<string, string[][]>> arrayObjects,
		string currentProperty)
	{
		var currentArray = arrayCurrentParents.First().Key;
		var currentObject = arrayCurrentParents.Last().Value.Last();

		var matchingObjects = arrayObjects
			.Where(o => o.Key == currentArray && o.Value[0].Last() == currentObject)
			.ToList();

		if (matchingObjects.Any(o => o.Value[1].Contains(currentProperty)))
		{
			return CompareLineage(arrayCurrentParents.Last().Value, matchingObjects);
		}

		return (false, null, null);
	}

	private static (bool, int?, List<KeyValuePair<string, string[][]>>?) CompareLineage(
		List<string> currentParents,
		List<KeyValuePair<string, string[][]>> matches)
	{
		for (int i = 0; i < matches.Count(); i++)
		{
			var compareLineage = matches[i].Value[0];

			for (int j = 0; j < compareLineage.Length; j++)
			{
				if (!compareLineage[j].Equals(currentParents[j]))
				{
					break;
				}

				if (j == currentParents.Count() - 1 &&
					compareLineage[j] == currentParents[j])
				{
					return (true, i, matches);
				}
			}
		}

		return (false, null, null);
	}
}