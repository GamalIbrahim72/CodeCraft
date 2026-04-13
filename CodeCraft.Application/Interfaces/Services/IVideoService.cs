using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Services;
 public interface IVideoService
{
    Task<string> UploadVideoAsync(Stream stream, string fileName);
}
