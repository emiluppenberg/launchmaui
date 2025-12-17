using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Android.Util;
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
  public static LaunchDetailsVM CreateFromJson(string rawContent)
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

    var bytes = Encoding.UTF8.GetBytes(rawContent);
    var reader = new Utf8JsonReader(bytes);

    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
      {
        modelCurrentParents.RemoveAt(modelCurrentParents.Count() - 1);
        continue;
      }

      if (reader.TokenType == JsonTokenType.PropertyName)
      {
        string? currentProperty = Regex.Replace(reader.GetString()!, @"(?:^|_)([a-z])", match => match.Groups[1].Value.ToUpper());
        reader.Read();

        if (reader.TokenType == JsonTokenType.StartArray || reader.TokenType == JsonTokenType.EndArray)
        {
          reader.Skip();
          continue;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
          modelCurrentParents.Add(currentProperty);
          continue;
        }

        var currentObject = modelCurrentParents.Last();
        var matchingObjects = modelObjects
          .Where(o => o.Key == currentObject)
          .ToList();

        if (!matchingObjects.Any(o => o.Value[1].Contains(currentProperty)))
        {
          continue;
        }

        var matchingIndex = 0;
        var isEqual = false;

        for (int i = 0; i < matchingObjects.Count(); i++)
        {
          var compareLineage = matchingObjects[i].Value[0];

          for (int j = 0; j < compareLineage.Length; j++)
          {
            if (!compareLineage[j].Equals(modelCurrentParents[j]))
            {
              break;
            }

            if (j == modelCurrentParents.Count() - 1 &&
              compareLineage[j] == modelCurrentParents[j])
            {
              matchingIndex = i;
              isEqual = true;
              Debug.WriteLine($"c-obj: {compareLineage[j]}, c-prop: {currentProperty}");
            }
          }
        }

        if (!isEqual)
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

        if (modelCurrentParents.Count() > 2)
        {
          var parentIndex = matchingObjects[matchingIndex].Value[0].Length - 2;
          currentObject = matchingObjects[matchingIndex].Value[0][parentIndex] + currentObject;
        }

        Debug.WriteLine($"obj: {currentObject}, prop: {currentProperty}, value: {value}");

        if (!model.ContainsKey(currentObject))
        {
          model.Add(currentObject, new Dictionary<string, object?> { [currentProperty] = value });
        }
        else
        {
          model[currentObject].Add(currentProperty, value);
        }
      }
    }

    foreach (var k in model.Keys)
    {
      Debug.WriteLine(k);
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

      MissionDescription = model.TryGetValue("Mission", out var mission)
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

      PadMapImageUrl = model.TryGetValue("Pad", out var pad)
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

      StatusDescription = model.TryGetValue("Status", out var status)
        && status.TryGetValue("Description", out var statusDescription)
        ? (string?)statusDescription
        : null,

      StatusName = status is not null && status.TryGetValue("Name", out var statusName)
        ? (string?)statusName
        : null,

      ThumbnailUrl = model.TryGetValue("Image", out var imageModel)
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
}