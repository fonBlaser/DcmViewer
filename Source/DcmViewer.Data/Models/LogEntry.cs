namespace DcmViewer.Data.Models;

public class LogEntry
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public Severity Severity { get; set; }
    public string Service { get; set; }
    public string Module { get; set; }
    public string Message { get; set; }
}