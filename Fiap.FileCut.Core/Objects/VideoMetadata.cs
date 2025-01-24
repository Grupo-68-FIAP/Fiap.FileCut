using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.Objects;

public class VideoMetadata(string name)
{
    public readonly string Name = name;
    public VideoState State { get; set; } = VideoState.PENDING;
}
