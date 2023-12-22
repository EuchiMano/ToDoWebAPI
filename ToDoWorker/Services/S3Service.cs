using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace ToDoWorker.Services
{
    public class S3Service
{
    private readonly string _bucketName;
    private readonly IAmazonS3 _s3Client;

    public S3Service(string accessKey, string secretKey, string bucketName, string region)
    {
        _bucketName = bucketName;
        _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
    }

    public async Task<string> UploadImageAsync(string key, Stream imageStream)
    {
        try
        {
            var fileTransferUtility = new TransferUtility(_s3Client);

            await fileTransferUtility.UploadAsync(imageStream, _bucketName, key);

            var url = GenerateS3Url(key);
            return url;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error uploading image to S3: {ex.Message}");
            throw;
        }
    }

    private string GenerateS3Url(string key)
    {
        return $"https://{_bucketName}.s3.amazonaws.com/{key}";
    }
}
}