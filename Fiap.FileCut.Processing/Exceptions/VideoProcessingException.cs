using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Processing.Exceptions
{

    public class VideoProcessingException : Exception
    {
    public VideoProcessingException() { }
    public VideoProcessingException(string message) : base(message) { }
    public VideoProcessingException(string message, Exception inner) : base(message, inner) { }
    }
}
