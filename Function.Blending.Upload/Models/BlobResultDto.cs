namespace Function.Blending.Upload.Models;
public class BlobResultDto
{
    public string FileName { get; set; } = default!;
    public string DownloadUrl { get; set; } = default!;
    public string Container { get; set; } = default!;
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public long SizeInBytes { get; set; }
    public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public List<RumaStockDisponibleDto>? ExcelDataSap { get; set; }
}