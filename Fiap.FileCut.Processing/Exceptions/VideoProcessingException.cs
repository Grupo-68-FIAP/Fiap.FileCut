using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Processing.Exceptions
{
    public class VideoProcessingException : Exception
    {
        public VideoProcessingException(string message, Exception innerException)
            : base(message, innerException) {
            //TODO
        }
    }
}
