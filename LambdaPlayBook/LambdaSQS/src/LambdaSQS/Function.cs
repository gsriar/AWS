using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaSQS
{
    public class Function
    {
        IAmazonS3 S3Client { get; set; }
        private string? BUCKET_NAME;
        private string? S3_KEY;
        private string? s3content;
        private const string BucketNameConst = "BUCKET_NAME";

        public Function()
        {
            S3Client = new AmazonS3Client();
            BUCKET_NAME = System.Environment.GetEnvironmentVariable(BucketNameConst);
            S3_KEY = DateTime.Now.ToString("queue/yy-MM(MMM)-dd HHmmssff").ToLower() + ".txt";
            s3content = $"The time is {DateTime.Now.ToString("MMM-dd (ddd) HHmmssff").ToLower()}";
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            
            StringBuilder sb = new StringBuilder();
            try
            {
                context.Logger.LogInformation($"Queue Message Body [{message.Body}]");

                sb.AppendLine($"BUCKET_NAME [{BUCKET_NAME}]");
                PutObjectRequest request = new PutObjectRequest();
                request.BucketName = BUCKET_NAME;
                request.Key = S3_KEY;
                request.ContentBody = message.Body;
                request.ContentType = "text/plain";

                sb.AppendLine($"Try Put S3");
                var response = S3Client.PutObjectAsync(request);
                sb.AppendLine($"S3 Action Create [{request.BucketName}] Key: [{request.Key}], response HttpStatusCode [{response.Result.HttpStatusCode}], VersionId [{response.Result.VersionId}]");
                sb.AppendLine($"Created S3 Object");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Exception [{ex.Message}]");
            }

            context.Logger.LogInformation(sb.ToString());


            // TODO: Do interesting work based on the new message
            await Task.CompletedTask;
        }
    }
}