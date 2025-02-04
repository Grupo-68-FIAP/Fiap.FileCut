using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.Interfaces.Services
{
    public interface IVideoProcessingService
    {
       Task ProcessVideoAsync(
        string videoPath,
        Guid userId,
        CancellationToken cancellationToken = default);
    }
}
