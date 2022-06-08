using Amazon.S3;
using Amazon.S3.Model;
using System.Text;

namespace SharedFunctions
{
    public class S3Helper
    {
        IAmazonS3 S3Client { get; set; }
        private string? BUCKET_NAME;
        private string s3Prefix;
        private const string BucketNameConst = "BUCKET_NAME";

        public S3Helper(string s3Prefix, IAmazonS3 s3Client)
        {
            s3Client = new AmazonS3Client();
            BUCKET_NAME = System.Environment.GetEnvironmentVariable(BucketNameConst);
            this.s3Prefix = s3Prefix;
            this.S3Client = s3Client;
        }

        public void CreateS3Object(string body, ref StringBuilder sb)
        {
            sb.AppendLine($"BUCKET_NAME [{BUCKET_NAME}]");
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = BUCKET_NAME;
            request.Key = $"{s3Prefix.ToLower()}-{DateTime.Now.ToString("yy-MM(MMM)-dd HHmmssff").ToLower()}.txt";
            request.ContentBody = body;
            request.ContentType = "text/plain";

            sb.AppendLine($"Try Put S3");
            var response = S3Client.PutObjectAsync(request);
            sb.AppendLine($"S3 Action Create [{request.BucketName}] Key: [{request.Key}], response HttpStatusCode [{response.Result.HttpStatusCode}], VersionId [{response.Result.VersionId}]");
            sb.AppendLine($"Created S3 Object");
        }

    }
}