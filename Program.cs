using DoubanBookScraper.Services;

namespace DoubanBookScraper;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== 豆瓣高分图书爬虫 ===\n");

        try
        {
            using var httpClientService = new HttpClientService();
            var scraperService = new BookScraperService(httpClientService);
            var exportService = new ExcelExportService();

            Console.WriteLine("开始爬取...\n");
            var (highScoreBooks, otherBooks, noRatingBooks, errors) =
                await scraperService.ScrapeAsync();

            Console.WriteLine($"\n=== 爬取完成 ===");
            Console.WriteLine($"高分图书 (≥9.0): {highScoreBooks.Count} 本");
            Console.WriteLine($"其他评分图书 (<9.0): {otherBooks.Count} 本");
            Console.WriteLine($"无评分图书: {noRatingBooks.Count} 本");
            Console.WriteLine($"错误数量: {errors.Count} 个\n");

            Console.WriteLine("=== 高分图书 Top 10 ===");
            foreach (var book in highScoreBooks.Take(10))
            {
                Console.WriteLine($"{book.Rating:F1} | {book.Tag} | {book.Title}");
            }

            exportService.Export(highScoreBooks, otherBooks, noRatingBooks, errors);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("操作被取消（超时或用户取消）。");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"程序发生未处理异常: {ex.Message}");
        }
    }
}