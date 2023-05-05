namespace VSGBulgariaMarketplace.Domain.Entities;

public class VersionInfo
{
    public long Version { get; set; }

    public DateTime? AppliedOn { get; set; }

    public string? Description { get; set; }
}