using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.Objects
{
    public class VideoProcessingConfig
    {
        public const string SectionName = "VideoProcessing";

        public TimeSpan IntervaloCaptura { get; set; } = TimeSpan.FromSeconds(20);
        public Size TamanhoFrame { get; set; } = new Size(1920, 1080);
    }
}
