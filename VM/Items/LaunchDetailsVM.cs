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
	string? rocketThumbnailUrl;

	[ObservableProperty]
	string? lspName; // LaunchServiceProvider -> Name

	[ObservableProperty]
	string? lspUrl;

	[ObservableProperty]
	string? lspThumbnailUrl; // LaunchServiceProvider -> SocialLogo -> ThumbnailUrl

	[ObservableProperty]
	string? padName; // Pad -> Name

	[ObservableProperty]
	string? padMapImageUrl; // Pad -> MapImage

	[ObservableProperty]
	string? padMapUrl;

	[ObservableProperty]
	string? padLocationName; // Pad -> Location -> Name

	[ObservableProperty]
	string? padLocationDescription;

	[ObservableProperty]
	string? url; // Url

	[ObservableProperty]
	DateTime? net; // Net

	[ObservableProperty]
	DateTime? windowStart; // WindowStart

	[ObservableProperty]
	DateTime? windowEnd; // WindowEnd

	[ObservableProperty]
	string? statusName;

	[ObservableProperty]
	string? statusAbbrev; // Status -> Abbrev

	[ObservableProperty]
	string? statusDescription; // Status -> Description

	[ObservableProperty]
	string? weatherConcerns; // WeatherConcerns

	[ObservableProperty]
	int?[]? launcherStageLauncherFlightNumber;

	[ObservableProperty]
	string?[]? launcherStageLauncherDetails;

	[ObservableProperty]
	DateTime?[]? launcherStageLauncherFirstLaunchDate;

	[ObservableProperty]
	DateTime?[]? launcherStageLauncherLastLaunchDate;

	[ObservableProperty]
	string?[]? launcherStageLandingLocationName;

	[ObservableProperty]
	string?[]? launcherStageLandingLocationDescription;

	[ObservableProperty]
	string?[]? launcherStageLandingLocationImageThumbnailUrl; // prop for LandingLocation image

	[ObservableProperty]
	string?[]? infoUrlsUrl;

	[ObservableProperty]
	string?[]? timelineRelativeTime;

	[ObservableProperty]
	string?[]? timelineTypeDescription;

	[ObservableProperty]
	string?[]? timelineTypeAbbrev;

	[ObservableProperty]
	string?[]? missionPatchesImageUrl;

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
				new("Image", [
				["Model", "Rocket", "Configuration", "Image"],
				["ThumbnailUrl"]]),
			new("LaunchServiceProvider",[
				["Model", "LaunchServiceProvider"],
				["Name", "Url"]]),
			new("SocialLogo", [
				["Model", "LaunchServiceProvider", "SocialLogo"],
				["ThumbnailUrl"]]),
			new("Pad", [
				["Model", "Pad"],
				["Name", "MapImage", "MapUrl"]]),
			new("Location", [
				["Model", "Pad", "Location"],
				["Name", "Description"]]),
			new("Status", [
				["Model", "Status"],
				["Abbrev", "Description" ,"Name"]])
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
				["FirstLaunchDate", "LastLaunchDate", "Details"]
			]),
			new("LauncherStage",[
				["Model", "Rocket", "LauncherStage", "Landing" ,"LandingLocation"],
				["Name", "Description"]
			]),
			new("LauncherStage",[
				["Model", "Rocket", "LauncherStage", "Landing" ,"LandingLocation", "Image"],
				["ThumbnailUrl"]
			]),
			new("InfoUrls",[
				["Model", "InfoUrls"],
				["Url"]
			]),
			new("Timeline",[
				["Model", "Timeline"],
				["RelativeTime"]
			]),
			new("Timeline",[
				["Model", "Timeline", "Type"],
				["Description", "Abbrev"]
			]),
			new("MissionPatches",[
				["Model", "MissionPatches"],
				["ImageUrl"]
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
					var lastIndex = arrayCurrentParents.IndexOf(last);

					if (last.Value.IndexOf(last.Key) >= lastIndex)
					{
						last.Value.RemoveAt(last.Value.Count() - 1);
					}

					continue;
				}

				if (reader.TokenType == JsonTokenType.EndArray)
				{
					arrayCurrentParents.RemoveAt(arrayCurrentParents.Count() - 1);

					if (arrayCurrentParents.Count() == 0)
					{
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
						var lineage = new List<string>(!isArray ? modelCurrentParents : arrayCurrentParents.Last().Value) { currentProperty };
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
						var nestedArrayIndex = arrayCurrentParents.Count() - 1;
						var nestedObjectIndex = arrayCurrentParents[nestedArrayIndex].Value.Count() - 1;
						var currentLevel = arrayCurrentParents[nestedArrayIndex];
						var isNestedObject = nestedObjectIndex > currentLevel.Value.IndexOf(currentLevel.Key);

						currentProperty = isNestedObject ? currentLevel.Value.Last() + currentProperty : currentProperty;
						currentObject = nestedArrayIndex > 0 ?
							arrayCurrentParents[nestedArrayIndex - 1].Key + currentLevel.Key :
							currentLevel.Value[currentLevel.Value.IndexOf(currentLevel.Key) - 1] + currentLevel.Key;
					}

					Debug.WriteLine($"{currentObject}.{currentProperty} - {value}");

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

			var launcherStageLandingLocationName = new List<string?>();
			var launcherStageLandingLocationDescription = new List<string?>();
			var launcherStageLandingLocationImageThumbnailUrl = new List<string?>();
			var launcherStageLauncherFlightNumber = new List<int?>();
			var launcherStageLauncherFirstLaunchDate = new List<DateTime?>();
			var launcherStageLauncherLastLaunchDate = new List<DateTime?>();
			var launcherStageLauncherDetails = new List<string?>();
			var infoUrlsUrl = new List<string?>();
			var timelineRelativeTime = new List<string?>();
			var timelineTypeDescription = new List<string?>();
			var timelineTypeAbbrev = new List<string?>();
			var missionPatchesImageUrl = new List<string?>();

			if (model.TryGetValue("RocketLauncherStage", out var launcherStage))
			{
				if (launcherStage.TryGetValue("LandingLocationName", out var landingLocationName))
				{
					CastToList<string?>(launcherStageLandingLocationName, landingLocationName as List<object?>);
				}

				if (launcherStage.TryGetValue("LandingLocationDescription", out var landingLocationDescription))
				{
					CastToList<string?>(launcherStageLandingLocationDescription, landingLocationDescription as List<object?>);
				}

				if (launcherStage.TryGetValue("ImageThumbnailUrl", out var imageThumbnailUrl))
				{
					CastToList<string?>(launcherStageLandingLocationImageThumbnailUrl, imageThumbnailUrl as List<object?>);
				}

				if (launcherStage.TryGetValue("LauncherFlightNumber", out var launcherFlightNumber))
				{
					CastToList<int?>(launcherStageLauncherFlightNumber, launcherFlightNumber as List<object?>);
				}

				if (launcherStage.TryGetValue("LauncherFirstLaunchDate", out var launcherFirstLaunchDate))
				{
					CastToList<DateTime?>(launcherStageLauncherFirstLaunchDate, launcherFirstLaunchDate as List<object?>);
				}

				if (launcherStage.TryGetValue("LauncherLastLaunchDate", out var launcherLastLaunchDate))
				{
					CastToList<DateTime?>(launcherStageLauncherLastLaunchDate, launcherLastLaunchDate as List<object?>);
				}
				if (launcherStage.TryGetValue("LauncherDetails", out var launcherDetails))
				{
					CastToList<string?>(launcherStageLauncherDetails, launcherDetails as List<object?>);
				}
			}

			if (model.TryGetValue("ModelInfoUrls", out var infoUrls))
			{
				if (infoUrls.TryGetValue("Url", out var urls))
				{
					CastToList<string?>(infoUrlsUrl, urls as List<object?>);
				}
			}

			if (model.TryGetValue("ModelTimeline", out var timeline))
			{
				if (timeline.TryGetValue("RelativeTime", out var relativeTime))
				{
					CastToList<string?>(timelineRelativeTime, relativeTime as List<object?>);
				}

				if (timeline.TryGetValue("TypeDescription", out var typeDescription))
				{
					CastToList<string?>(timelineTypeDescription, typeDescription as List<object?>);
				}

				if (timeline.TryGetValue("TypeAbbrev", out var typeAbbrev))
				{
					CastToList<string?>(timelineTypeAbbrev, typeAbbrev as List<object?>);
				}
			}

			if (model.TryGetValue("ModelMissionPatches", out var missionPatches))
			{
				if (missionPatches.TryGetValue("ImageUrl", out var patchImageUrls))
				{
					CastToList<string?>(missionPatchesImageUrl, patchImageUrls as List<object?>);
				}
			}


			return new LaunchDetailsVM
			{
				LauncherStageLandingLocationName = launcherStageLandingLocationName.Count > 0 ?
					launcherStageLandingLocationName.ToArray() : null,

				LauncherStageLandingLocationDescription = launcherStageLandingLocationDescription.Count > 0 ?
					launcherStageLandingLocationDescription.ToArray() : null,

				LauncherStageLandingLocationImageThumbnailUrl = launcherStageLandingLocationImageThumbnailUrl.Count > 0 ?
					launcherStageLandingLocationImageThumbnailUrl.ToArray() : ["fallback.jpg"],

				LauncherStageLauncherFlightNumber = launcherStageLauncherFlightNumber.Count > 0 ?
					launcherStageLauncherFlightNumber.ToArray() : null,

				LauncherStageLauncherFirstLaunchDate = launcherStageLauncherFirstLaunchDate.Count > 0 ?
					launcherStageLauncherFirstLaunchDate.ToArray() : null,

				LauncherStageLauncherLastLaunchDate = launcherStageLauncherLastLaunchDate.Count > 0 ?
					launcherStageLauncherLastLaunchDate.ToArray() : null,

				InfoUrlsUrl = infoUrlsUrl.Count > 0 ?
					infoUrlsUrl.ToArray() : null,

				TimelineRelativeTime = timelineRelativeTime.Count > 0 ?
					timelineRelativeTime.ToArray() : null,

				TimelineTypeDescription = timelineTypeDescription.Count > 0 ?
					timelineTypeDescription.ToArray() : null,

				TimelineTypeAbbrev = timelineTypeAbbrev.Count > 0 ?
					timelineTypeAbbrev.ToArray() : null,

				MissionPatchesImageUrl = missionPatchesImageUrl.Count > 0 ?
					missionPatchesImageUrl.ToArray() : ["fallback.jpg"],

				LspName = model.TryGetValue("ModelLaunchServiceProvider", out var lsp)
						&& lsp.TryGetValue("Name", out var lspName)
						? (string?)lspName
						: null,

				LspUrl = lsp is not null && lsp.TryGetValue("Url", out var lspUrl)
						? (string?)lspUrl
						: null,

				LspThumbnailUrl = model.TryGetValue("LaunchServiceProviderSocialLogo", out var socialLogo)
					&& socialLogo.TryGetValue("ThumbnailUrl", out var lspThumbnailUrl)
					? (string?)lspThumbnailUrl
					: "fallback.jpg",

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

				PadLocationDescription = location is not null && location.TryGetValue("Description", out var padLocationDescription)
					? (string?)padLocationDescription
					: null,

				PadMapImageUrl = model.TryGetValue("ModelPad", out var pad)
					&& pad.TryGetValue("MapImage", out var padMapImage)
					? (string?)padMapImage
					: "fallback.jpg",

				PadMapUrl = pad is not null && pad.TryGetValue("MapUrl", out var padMapUrl)
					? (string?)padMapUrl
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

				RocketThumbnailUrl = model.TryGetValue("ConfigurationImage", out var configurationImage)
					&& configurationImage.TryGetValue("ThumbnailUrl", out var configurationThumbnailUrl)
					? (string?)configurationThumbnailUrl
					: "fallback.jpg",

				StatusDescription = model.TryGetValue("ModelStatus", out var status)
					&& status.TryGetValue("Description", out var statusDescription)
					? (string?)statusDescription
					: null,

				StatusAbbrev = status is not null && status.TryGetValue("Abbrev", out var statusAbbrev)
					? (string?)statusAbbrev
					: null,

				StatusName = status is not null && status.TryGetValue("Name", out var statusName)
					? (string?)statusName
					: null,

				// ThumbnailUrl = model.TryGetValue("ModelImage", out var imageModel)
				// 	&& imageModel.TryGetValue("ThumbnailUrl", out var thumbnailUrl)
				// 	? (string?)thumbnailUrl
				// 	: null,

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

				Title = $"{(model.TryGetValue("ModelLaunchServiceProvider", out var titleLsp)
							&& titleLsp.TryGetValue("Name", out var titleLspName)
							? (string?)titleLspName
							: "")} | {(model.TryGetValue("ModelMission", out var titleMission)
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
			.Where(o => o.Key == currentObject && o.Value[0].SequenceEqual(modelCurrentParents))
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
		var last = arrayCurrentParents.Last();
		var currentArray = last.Key;
		var currentObject = last.Value.Last();

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

	private static void CastToList<T>(List<T?> listToAdd, List<object?>? listToCast)
	{
		if (listToCast is not null)
		{
			foreach (var o in listToCast)
			{
				if (o is T value)
				{
					listToAdd.Add(value);
					continue;
				}

				listToAdd.Add(default);
			}
		}
	}
}