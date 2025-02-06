using Fiap.FileCut.Core.Objects.Enums;

namespace Fiap.FileCut.Core.Objects;

public readonly record struct VideoMetadata
{

    public VideoMetadata(string name)
    {
        Name = name;
    }

    public VideoMetadata(string name, VideoState state)
    {
        Name = name;
        State = state;
    }

    public string Name { get; }
    public VideoState State { get; } = VideoState.PENDING;
}
