namespace DoubanBookScraper.Models;

/// <summary>
/// 图书实体类
/// </summary>
public class Book
{
    public string Title { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is Book book &&
               (Url == book.Url || Title.Equals(book.Title, StringComparison.Ordinal));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Url, Title);
    }
}