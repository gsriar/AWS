using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaSNS
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
            s3helper = new SharedFunctions.S3Helper("SNS", new AmazonS3Client());
           
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SNSEvent evnt, ILambdaContext context)
        {
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsyncProcessRecordAsync(message, context);
            }
        }

        private async Task ProcessMessageAsyncProcessRecordAsync(SNSEvent.SNSRecord record, ILambdaContext context)
        {
            context.Logger.LogInformation($"Processed record {record.Sns.Message}");

            StringBuilder sb = new StringBuilder();
            try
            {
                context.Logger.LogInformation($"Queue Message Body [{record.Sns.Message}]");
                s3helper.CreateS3Object(record.Sns.Message, ref sb);
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