using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Util;
using SharedFucntions;
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
            s3helper = new SharedFunctions.S3Helper("S3", Constants.S3LogBucket, new AmazonS3Client());
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
            string msg = JsonHelper.JsonSerialize2<S3Event>(evnt);
            StringBuilder sb = new StringBuilder();
            try
            {
                s3helper.CreateS3Object(msg, ref sb);
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Exception [{ex.Message}]");
            }

            context.Logger.LogInformation(sb.ToString());
            await Task.CompletedTask;
            return  sb.ToString();
        }
    }
}