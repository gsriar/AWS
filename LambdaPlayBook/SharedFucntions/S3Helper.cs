using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using SharedFucntions;
using System.Text;

namespace SharedFunctions
{
    public class S3Helper
    {
        IAmazonS3 S3Client { get; set; }
        public string? bucketName { get; private set; }

        private string s3Prefix;
      
        public S3Helper(string s3Prefix,string bucketName, IAmazonS3 s3Client)
        {
            s3Client = new AmazonS3Client();
            this.bucketName = bucketName;
            this.s3Prefix = s3Prefix;
            this.S3Client = s3Client;
        }
        public void ReadS3Object(string objectKey, ref StringBuilder sb)
        {
            sb.AppendLine($"S3Helper BUCKET_NAME [{bucketName}]");

            TransferUtility tr = new TransferUtility(S3Client);

            var file = $"/tmp/{Guid.NewGuid().ToString()}.txt";

            TransferUtilityDownloadRequest downloadRequest = new TransferUtilityDownloadRequest() {
            BucketName=bucketName,
            Key=objectKey,
            FilePath= file
            };
            tr.DownloadAsync(downloadRequest);

            sb.AppendLine("****");
            var txt = System.IO.File.ReadAllText(file);


            sb.AppendLine(txt.Substring(0, txt.Length > 50 ? 50 : txt.Length));

            sb.AppendLine("****");
        }

        public void CreateS3Object(string body, ref StringBuilder sb)
        {
            sb.AppendLine($"S3Helper BUCKET_NAME [{bucketName}]");
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = bucketName;
            request.Key = $"{s3Prefix.ToLower()}-{DateTime.Now.ToString("yy-MM(MMM)-dd HHmmssff").ToLower()}.txt";
            request.ContentBody = body;
            request.ContentType = "text/plain";

            sb.AppendLine($"Try Put S3");
            var response = S3Client.PutObjectAsync(request);
            sb.AppendLine($"S3 Action Create [{request.BucketName}] Key: [{request.Key}], response HttpStatusCode [{response.Result.HttpStatusCode}], VersionId [{response.Result.VersionId}]");
            sb.AppendLine($"Created S3 Object");
        }

        public void CreateS3Object(string namePrefix, string body, ref StringBuilder sb)
        {
            sb.AppendLine($"BUCKET_NAME [{bucketName}]");
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = bucketName;
            request.Key = $"{s3Prefix.ToLower()}-{namePrefix}-{DateTime.Now.ToString("yy-MM(MMM)-dd HHmmssff").ToLower()}.txt";
            request.ContentBody = body;
            request.ContentType = "text/plain";

            sb.AppendLine($"Try Put S3");
            var response = S3Client.PutObjectAsync(request);
            sb.AppendLine($"S3 Action Create [{request.BucketName}] Key: [{request.Key}], response HttpStatusCode [{response.Result.HttpStatusCode}], VersionId [{response.Result.VersionId}]");
            sb.AppendLine($"Created S3 Object");
        }

    }
}