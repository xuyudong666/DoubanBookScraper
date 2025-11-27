namespace DoubanBookScraper.Configuration;

/// <summary>
/// 爬虫配置类
/// </summary>
public class ScraperConfig
{
    /// <summary>
    /// 最低评分阈值
    /// </summary>
    public const double MinRating = 9.0;

    /// <summary>
    /// 每页显示数量
    /// </summary>
    public const int PageSize = 20;

    /// <summary>
    /// 最大爬取页数
    /// </summary>
    public const int MaxPages = 10;

    /// <summary>
    /// HTTP请求重试次数
    /// </summary>
    public const int MaxRetryAttempts = 3;

    /// <summary>
    /// 请求延迟最小值（毫秒）
    /// </summary>
    public const int MinDelayMs = 300;

    /// <summary>
    /// 请求延迟最大值（毫秒）
    /// </summary>
    public const int MaxDelayMs = 800;

    /// <summary>
    /// 搜索标签列表
    /// </summary>
    public static readonly string[] Tags =
    {
        "计算机", "计算机科学", "计算机基础", "计算机原理", "计算机组成原理", "计算机体系结构",
        "操作系统", "Linux", "Unix", "Windows", "macOS",
        "软件工程", "软件设计", "系统分析与设计", "需求分析", "UML", "设计模式",
        "计算机网络", "TCP/IP", "网络安全", "网络协议",
        "算法", "数据结构", "算法与数据结构", "算法设计与分析",
        "编程", "程序设计", "C语言", "C++", "Java", "Python", "JavaScript", "Go", "Rust", "PHP", "Swift", "Kotlin", "Ruby",
        "数据库", "MySQL", "PostgreSQL", "MongoDB", "SQL", "NoSQL",
        //"人工智能", "机器学习", "深度学习", "自然语言处理", "计算机视觉",
        //"数字逻辑", "信息安全", "密码学", "并行计算", "分布式系统", "云计算", "大数据", "区块链"
    };
}