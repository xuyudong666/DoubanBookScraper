using DoubanBookScraper.Configuration;

namespace DoubanBookScraper.Services;

/// <summary>
/// HTTP客户端服务实现
/// </summary>
public class HttpClientService : IHttpClientService, IDisposable
{
    private readonly HttpClient _client;

    public HttpClientService()
    {
        _client = new HttpClient
        {
            Timeout = TimeSpan.FromHours(15)
        };
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        _client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml");
        _client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
    }

    public async Task<string?> GetHtmlWithRetryAsync(
        string url,
        string tag,
        List<(string Tag, string Url, string Error)> errors,
        CancellationToken token = default)
    {
        for (int attempt = 1; attempt <= ScraperConfig.MaxRetryAttempts; attempt++)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                return await _client.GetStringAsync(url, token);
            }
            catch (HttpRequestException ex)
            {
                errors.Add((tag, url, $"HTTP异常(第{attempt}次): {ex.Message}"));
            }
            catch (TaskCanceledException ex)
            {
                string msg = ex.CancellationToken.IsCancellationRequested
                    ? "请求被取消"
                    : "请求超时";
                errors.Add((tag, url, $"{msg}(第{attempt}次): {ex.Message}"));
            }
            catch (Exception ex)
            {
                errors.Add((tag, url, $"未知异常(第{attempt}次): {ex.Message}"));
            }

            await Task.Delay(300 * attempt * attempt, token);
        }
        return null;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}