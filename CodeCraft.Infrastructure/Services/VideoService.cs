using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Services;
public class VideoService: IVideoService
{
    private readonly Cloudinary _cloudinary;

    public VideoService(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadVideoAsync(Stream stream, string fileName)
    {
        if (stream == null || stream.Length == 0)
            throw new Exception("Video is empty");

        var uploadParams = new VideoUploadParams
        {
            File = new FileDescription(fileName, stream)
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new Exception(result.Error.Message);

        return result.SecureUrl.ToString();
    }
}
