using System.Web;
using AibolitAPI.Interfaces;
using Google.Cloud.Storage.V1;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace AibolitAPI.Services;

public class GoogleCloudStorageService : ICloudStorageService
{
    private readonly string _bucketName;
    private readonly StorageClient _storageClient;

    public GoogleCloudStorageService(IConfiguration configuration)
    {
        _bucketName = configuration["GoogleCloud:BucketName"]
                      ?? throw new ArgumentNullException("GoogleCloud:BucketName is not configured.");
        _storageClient = StorageClient.Create();
    }

    public async Task<string> UploadPhotoAsync(Stream stream, string fileName)
    {
        if (!IsImageFile(fileName))
            throw new InvalidOperationException("Only .jpg and .jpeg formats are allowed.");

        try
        {
            var fileNameWithoutPath = Path.GetFileName(fileName);
            await using var compressedStream = await CompressImageAsync(stream);

            var objectName = $"{Guid.NewGuid()}_{fileNameWithoutPath}";
            var contentType = GetContentType(fileName);

            var obj = await _storageClient.UploadObjectAsync(new Object
            {
                Bucket = _bucketName,
                Name = objectName,
                ContentType = contentType
            }, compressedStream);

            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error uploading photo to Google Cloud Storage.", ex);
        }
    }


    public async Task DeletePhotoAsync(string fileUrl)
    {
        try
        {
            var uri = new Uri(fileUrl);
            var objectName = HttpUtility.UrlDecode(uri.AbsolutePath.TrimStart('/'));
            objectName = RemoveBucketPrefix(objectName);
            Console.WriteLine($"Attempting to delete object: {objectName}");
            await _storageClient.DeleteObjectAsync(_bucketName, objectName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error details: {ex.Message}");
            throw new InvalidOperationException("Error deleting photo from Google Cloud Storage.", ex);
        }
    }


    private string RemoveBucketPrefix(string objectName)
    {
        const string bucketPrefix = "aibolit-bucket/";

        return objectName.StartsWith(bucketPrefix)
            ? objectName.Substring(bucketPrefix.Length)
            : objectName;
    }


    private bool IsImageFile(string fileName)
    {
        var validExtensions = new[] { ".jpg", ".jpeg" };
        var extension = Path.GetExtension(fileName).ToLower();
        return validExtensions.Contains(extension);
    }

    private string GetContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
    }

    private async Task<Stream> CompressImageAsync(Stream inputStream)
    {
        using var image = await Image.LoadAsync(inputStream);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(1024, 1024)
        }));

        var outputStream = new MemoryStream();
        var encoder = new JpegEncoder { Quality = 90 };
        await image.SaveAsync(outputStream, encoder);

        outputStream.Seek(0, SeekOrigin.Begin);
        return outputStream;
    }
}