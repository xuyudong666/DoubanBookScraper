using DoubanBookScraper.Models;


/// <summary>
/// 图书爬虫服务接口
/// </summary>
public interface IBookScraperService
{
    Task<(List<Book> HighScoreBooks, List<Book> OtherBooks,
          List<(string Tag, string Title, string Url)> NoRatingBooks,
          List<(string Tag, string Url, string Error)> Errors)> ScrapeAsync(
        CancellationToken token = default);
}