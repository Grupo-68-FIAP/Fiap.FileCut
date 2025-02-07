using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.Interfaces.Services
{
    public interface IPackageService
    {
        Task<string> PackageImagesAsync(string filePath);
    }
}
