namespace Fiap.FileCut.Core.Objects;

public class ProcessingStatus
{
    public Guid UserId { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}
