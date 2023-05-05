namespace VSGBulgariaMarketplace.Domain.Entities;

public class Log
{
    public int Id { get; set; }

    public string Level { get; set; } = null!;

    public string CallSite { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string StackTrace { get; set; } = null!;

    public string InnerException { get; set; } = null!;

    public string AdditionalInfo { get; set; } = null!;

    public DateTime LoggedOnDate { get; set; }
}
