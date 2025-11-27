using System.Globalization;
using System.Net;
using DoubanBookScraper.Configuration;
using DoubanBookScraper.Models;
using HtmlAgilityPack;

namespace DoubanBookScraper.Services;

/// <summary>
/// 图书爬虫服务实现
/// </summary>
public class BookScraperService : IBookScraperService
{
    private readonly IHttpClientService _httpClientService;

    public BookScraperService(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public async Task<(List<Book> HighScoreBooks, List<Book> OtherBooks,
          List<(string Tag, string Title, string Url)> NoRatingBooks,
          List<(string Tag, string Url, string Error)> Errors)> ScrapeAsync(
        CancellationToken token = default)
    {
        var books = new HashSet<Book>();
        var otherBooks = new HashSet<Book>();
        var noRatingBooks = new List<(string Tag, string Title, string Url)>();
        var errors = new List<(string Tag, string Url, string Error)>();

        foreach (var tag in ScraperConfig.Tags)
        {
            Console.WriteLine($"当前tag: {tag}");
            await ScrapeTagAsync(tag, books, otherBooks, noRatingBooks, errors, token);
        }

        return (
            books.OrderByDescending(b => b.Rating).ToList(),
            otherBooks.OrderByDescending(b => b.Rating).ToList(),
            noRatingBooks,
            errors
        );
    }

    private async Task ScrapeTagAsync(
        string tag,
        HashSet<Book> books,
        HashSet<Book> otherBooks,
        List<(string Tag, string Title, string Url)> noRatingBooks,
        List<(string Tag, string Url, string Error)> errors,
        CancellationToken token)
    {
        int maxStart = ScraperConfig.MaxPages * ScraperConfig.PageSize;

        for (int start = 0; start < maxStart; start += ScraperConfig.PageSize)
        {
            string url = $"https://book.douban.com/tag/{WebUtility.UrlEncode(tag)}?start={start}&type=T";
            string? html = await _httpClientService.GetHtmlWithRetryAsync(url, tag, errors, token);

            if (html == null)
                continue;

            if (!ParseHtmlPage(html, tag, url, books, otherBooks, noRatingBooks, errors))
                break;

            await Task.Delay(Random.Shared.Next(ScraperConfig.MinDelayMs, ScraperConfig.MaxDelayMs), token);
        }
    }

    private bool ParseHtmlPage(
        string html,
        string tag,
        string url,
        HashSet<Book> books,
        HashSet<Book> otherBooks,
        List<(string Tag, string Title, string Url)> noRatingBooks,
        List<(string Tag, string Url, string Error)> errors)
    {
        var doc = new HtmlDocument();

        try
        {
            doc.LoadHtml(html);
        }
        catch (Exception ex)
        {
            errors.Add((tag, url, "LoadHtml异常: " + ex.Message));
            return false;
        }

        var nodes = doc.DocumentNode.SelectNodes("//li[@class='subject-item']");
        if (nodes == null || nodes.Count == 0)
            return false;

        foreach (var node in nodes)
        {
            ProcessBookNode(node, tag, url, books, otherBooks, noRatingBooks, errors);
        }

        return true;
    }

    private void ProcessBookNode(
        HtmlNode node,
        string tag,
        string url,
        HashSet<Book> books,
        HashSet<Book> otherBooks,
        List<(string Tag, string Title, string Url)> noRatingBooks,
        List<(string Tag, string Url, string Error)> errors)
    {
        try
        {
            var titleNode = node.SelectSingleNode(".//h2/a");
            if (titleNode == null)
                return;

            var ratingNode = node.SelectSingleNode(".//span[@class='rating_nums']");
            string title = titleNode.InnerText.Trim();
            string urlDetail = titleNode.GetAttributeValue("href", "").Trim();

            var book = new Book
            {
                Title = title,
                Url = urlDetail,
                Tag = tag
            };

            bool isDuplicate = books.Contains(book) || otherBooks.Contains(book);
            if (isDuplicate)
                return;

            if (ratingNode == null)
            {
                noRatingBooks.Add((tag, title, urlDetail));
                return;
            }

            string ratingText = ratingNode.InnerText.Trim();
            if (double.TryParse(ratingText, NumberStyles.Float, CultureInfo.InvariantCulture, out double rating))
            {
                book.Rating = rating;

                if (rating >= ScraperConfig.MinRating)
                {
                    books.Add(book);
                }
                else
                {
                    otherBooks.Add(book);
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add((tag, url, "节点处理异常: " + ex.Message));
        }
    }
}