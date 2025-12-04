using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<string> Upload(IFormFile file, string folderName);
        Task<bool> Delete(string filePath);
    }
}
