using System.Diagnostics;
using launchmaui.Utilities;

namespace launchmaui.Services;

public class ImageValidationService(HttpClient httpClient)
{
  private readonly HttpClient httpClient = httpClient;
  private readonly SemaphoreSlim semaphore = new(10, 10);

  public async Task<string> ValidateImageUrl(string? url)
  {
    if (!string.IsNullOrEmpty(url))
    {
      try
      {
        await semaphore.WaitAsync();
        using var request = new HttpRequestMessage(HttpMethod.Head, url);
        using var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
          var contentType = response.Content.Headers.ContentType?.MediaType;

          if (contentType?.StartsWith("image/", StringComparison.OrdinalIgnoreCase) == true)
          {
            return url;
          }
        }
      }
      catch (HttpRequestException ex)
      {
        Debug.WriteLine($"HTTP error for {url}: {ex.Message}");
      }
      catch (TaskCanceledException)
      {
        Debug.WriteLine($"Timeout for {url}");
      }
      catch (ObjectDisposedException ex)
      {
        Debug.WriteLine($"Object disposed for {url}: {ex.Message}");
      }
      finally
      {
        semaphore.Release();
      }
    }

    return General.placeholderUrl;
  }
}