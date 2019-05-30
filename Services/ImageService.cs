using Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Services
{
    public class ImageService : IImageService
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ImageService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        //metoda zwracajaca wskazany obraz
        public async Task<FileStream> GetImage(string filename)
        {
            var stream = _hostingEnvironment.WebRootPath + "\\uploads\\" + filename;
            var imageFileStream = File.OpenRead(stream);
            return imageFileStream;
        }
    }
}
