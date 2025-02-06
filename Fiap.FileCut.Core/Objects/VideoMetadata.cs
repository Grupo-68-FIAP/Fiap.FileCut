using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.Objects;

public class VideoMetadata
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
