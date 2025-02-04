public class ProcessingOptions
{
    public const string SectionName = "Processing";
    
    public string WorkingDirectory { get; set; } = "processing";
    public int FrameIntervalSeconds { get; set; } = 20;
    public int FrameWidth { get; set; } = 1920;
    public int FrameHeight { get; set; } = 1080;
    public string ZipFileNameFormat { get; set; } = "frames_{userId}_{processId}";
}