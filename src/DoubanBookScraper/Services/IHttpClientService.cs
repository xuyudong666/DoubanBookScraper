namespace DoubanBookScraper.Services;

/// <summary>
/// HTTP客户端服务接口
/// </summary>
public interface IHttpClientService
{
    Task<string?> GetHtmlWithRetryAsync(
        string url,
        string tag,
        List<(string Tag, string Url, string Error)> errors,
        CancellationToken token = default);
}