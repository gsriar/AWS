using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Util;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaS3Trigger
{
    public class Function
    {
        StringBuilder sb = new StringBuilder();
        SharedFunctions.S3Helper s3helper;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            s3helper = new SharedFunctions.S3Helper("S3", new AmazonS3Client());
        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string?> FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            sb.Clear();
            var s3Event = evnt.Records?[0].S3;
            if (s3Event == null)
            {
                sb.AppendLine("No S3 Event data");
            }
            try
            {
                var response = $"Bucket: {s3Event?.Bucket.Name}, Key : {s3Event?.Object.Key}";
                s3helper.CreateS3Object(response, ref sb);

            }
            catch (Exception e)
            {
                sb.AppendLine($"Exception: {e.Message}");
            }
            context.Logger.LogInformation(sb.ToString());
            return sb.ToString();
        }
    }
}