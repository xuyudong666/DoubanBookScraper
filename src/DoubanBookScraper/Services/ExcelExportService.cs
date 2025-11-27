using ClosedXML.Excel;
using DoubanBookScraper.Models;

namespace DoubanBookScraper.Services;

/// <summary>
/// Excel导出
/// </summary>
public class ExcelExportService
{
    public void Export(
        List<Book> highScoreBooks,
        List<Book> otherBooks,
        List<(string Tag, string Title, string Url)> noRatingBooks,
        List<(string Tag, string Url, string Error)> errors,
        string? outputPath = null)
    {
        string filePath = outputPath ?? $"豆瓣图书_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        using var workbook = new XLWorkbook();

        CreateHighScoreBooksSheet(workbook, highScoreBooks);
        CreateOtherBooksSheet(workbook, otherBooks);
        CreateNoRatingBooksSheet(workbook, noRatingBooks);
        CreateErrorsSheet(workbook, errors);

        workbook.SaveAs(filePath);
        Console.WriteLine($"Excel 导出完成: {filePath}");
    }

    private void CreateHighScoreBooksSheet(XLWorkbook workbook, List<Book> books)
    {
        var worksheet = workbook.AddWorksheet("高分图书");
        AddBookHeaders(worksheet);
        
        for (int i = 0; i < books.Count; i++)
        {
            AddBookRow(worksheet, books[i], i + 2);
        }
        
        FormatWorksheet(worksheet, 4);
    }

    private void CreateOtherBooksSheet(XLWorkbook workbook, List<Book> books)
    {
        var worksheet = workbook.AddWorksheet("其他评分图书");
        AddBookHeaders(worksheet);
        
        for (int i = 0; i < books.Count; i++)
        {
            AddBookRow(worksheet, books[i], i + 2);
        }
        
        FormatWorksheet(worksheet, 4);
    }

    private void CreateNoRatingBooksSheet(
        XLWorkbook workbook,
        List<(string Tag, string Title, string Url)> books)
    {
        var worksheet = workbook.AddWorksheet("无评分图书");
        worksheet.Cell(1, 1).Value = "标签";
        worksheet.Cell(1, 2).Value = "标题";
        worksheet.Cell(1, 3).Value = "链接";
        
        for (int i = 0; i < books.Count; i++)
        {
            var (tag, title, url) = books[i];
            worksheet.Cell(i + 2, 1).Value = tag;
            worksheet.Cell(i + 2, 2).Value = title;
            worksheet.Cell(i + 2, 3).Value = url;
        }
        
        FormatWorksheet(worksheet, 3);
    }

    private void CreateErrorsSheet(
        XLWorkbook workbook,
        List<(string Tag, string Url, string Error)> errors)
    {
        var worksheet = workbook.AddWorksheet("错误日志");
        worksheet.Cell(1, 1).Value = "标签";
        worksheet.Cell(1, 2).Value = "URL";
        worksheet.Cell(1, 3).Value = "错误信息";
        
        for (int i = 0; i < errors.Count; i++)
        {
            var (tag, url, error) = errors[i];
            worksheet.Cell(i + 2, 1).Value = tag;
            worksheet.Cell(i + 2, 2).Value = url;
            worksheet.Cell(i + 2, 3).Value = error;
        }
        
        FormatWorksheet(worksheet, 3);
    }

    private void AddBookHeaders(IXLWorksheet worksheet)
    {
        worksheet.Cell(1, 1).Value = "标签";
        worksheet.Cell(1, 2).Value = "标题";
        worksheet.Cell(1, 3).Value = "评分";
        worksheet.Cell(1, 4).Value = "链接";
    }

    private void AddBookRow(IXLWorksheet worksheet, Book book, int row)
    {
        worksheet.Cell(row, 1).Value = book.Tag;
        worksheet.Cell(row, 2).Value = book.Title;
        worksheet.Cell(row, 3).Value = book.Rating;
        worksheet.Cell(row, 4).Value = book.Url;
    }

    private void FormatWorksheet(IXLWorksheet worksheet, int columnCount)
    {
        worksheet.Range(1, 1, 1, columnCount).Style.Font.Bold = true;
        worksheet.Columns().AdjustToContents();
    }
}