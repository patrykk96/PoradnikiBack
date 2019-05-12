using Data.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IImageService
    {
        Task<FileStream> GetImage(string filename);
    }
}
