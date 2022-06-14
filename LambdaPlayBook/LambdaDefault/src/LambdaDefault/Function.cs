using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using SharedFucntions;
using SharedFunctions;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaDefault
{

    public class Function
    {

        SharedFunctions.S3Helper s3helper = null;
        private S3Helper s3Log;
        private const string CaseRDS = "RDS";
        private const string CaseS3Create = "S3-CREATE";
        private const string CaseS3Read = "S3-READ";
        private const string CaseS3Delete = "S3-DELETE";
        private const string CaseSNSEMAIL = "SNS-EMAIL";
        private const string CaseSNSJSON = "SNS-JSON";
        private const string CaseLOG = "LOG";
        private const string CaseTempFile = "TEMP-FILE";

        private const string EnvBucketNameConst = "BUCKET_NAME";
        private const string EnvTopicARNConst = "SNS_TOPIC_ARN";
        private const string EnvConnStringConst = "CONNECTION_STRING";
        private const string EnvLogBucketNameConst = "LOG_BUCKET_NAME";

        private string? SNS_TOPIC_ARN;
        
       
        private AmazonSimpleNotificationServiceClient snsClient;

        IAmazonS3 S3Client { get; set; }
        private string? BUCKET_NAME;
        private string? S3_KEY;
        private string? s3content;

        public Function()
        {
            S3Client = new AmazonS3Client();

            s3helper = new SharedFunctions.S3Helper("default", S3Client);

            s3Log = new SharedFunctions.S3Helper("default-log-", EnvLogBucketNameConst, S3Client);

            BUCKET_NAME = System.Environment.GetEnvironmentVariable(EnvBucketNameConst);
            S3_KEY = DateTime.Now.ToString("yy-MM(MMM)-dd HHmmssff").ToLower() + ".txt";
            s3content = $"The time is {DateTime.Now.ToString("MMM-dd (ddd) HHmmssff").ToLower()}";

            SNS_TOPIC_ARN = System.Environment.GetEnvironmentVariable(EnvTopicARNConst);
            snsClient = new AmazonSimpleNotificationServiceClient();
        }

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            StringBuilder sb=new StringBuilder();
            try
            {
                sb.AppendLine(String.Join(" | ", "Expected Environment Variables", EnvBucketNameConst, EnvTopicARNConst, EnvConnStringConst));
                var result = (input ?? "No Input");

                var args = result.Split(':');

                args[0] = args[0].ToUpper();

                var commmand = args[0];

                sb.AppendLine($"commnand [{commmand}]");

                string? firstText = null;
                if (args.Length > 1)
                    firstText = args[1];

                string? secondText = null;
                if (args.Length > 2)
                    secondText = args[2];

                sb.AppendLine($"content [{firstText}]");

                sb.AppendLine(String.Join("|", args));

                switch (commmand)
                {
                    case CaseS3Create:
                        {
                            s3helper.CreateS3Object(s3content??"", ref sb);
                        }
                        break;

                    case CaseS3Delete:
                        {
                            sb.AppendLine($"Try delete S3 - DeleteObjectAsync ");
                            var response = S3Client.DeleteObjectAsync(BUCKET_NAME, firstText);

                            sb.AppendLine($"S3 Action Delete [{BUCKET_NAME}] Key: [{firstText}], response HttpStatusCode [{response.Result.HttpStatusCode}], VersionId [{response.Result.VersionId}]");
                            sb.AppendLine($"delete S3 Object , HttpStatusCode [{response.Result.HttpStatusCode}]");
                        }
                        break;

                    case CaseS3Read:
                        {
                            sb.AppendLine($"Try download from s3 ");
                            s3Log.ReadS3Object(args[1], ref sb);
                            sb.AppendLine($"Done ");
                        }
                        break;
                    case CaseSNSEMAIL:
                        {
                            sb.AppendLine($"SNS_TOPIC_ARN [{SNS_TOPIC_ARN}]");
                            PublishRequest publishReq = new PublishRequest()
                            {
                                TargetArn = SNS_TOPIC_ARN,
                                Subject = $"{(firstText ?? $"No Subject")} [{DateTime.Now.ToString("MMM-dd HH:mm:ss")}]",
                                Message = $"{(secondText ?? "No Email Body Content")} [{DateTime.Now.ToString("MMM-dd HH:mm:ss")}]",
                            };

                            publishReq.MessageAttributes.Add("mission", new MessageAttributeValue { StringValue = "ga-aws-email-notification", DataType = "String" });

                            s3Log.CreateS3Object("PublishRequest", JsonHelper.JsonSerialize2<PublishRequest>(publishReq), ref sb);

                            sb.AppendLine($"Try publish SNSEMAIL message");
                            var response =  snsClient.PublishAsync(publishReq);
                            sb.AppendLine("SNS Message Pushed");

                            sb.AppendLine($"SNS[{publishReq.TargetArn}]Message [{publishReq.Message}] created, response HttpStatusCode [{response.Result.HttpStatusCode}]");
                            sb.AppendLine($"published SNS message");
                        }
                        break;

                    case CaseSNSJSON:
                        {
                            sb.AppendLine($"SNS_TOPIC_ARN [{SNS_TOPIC_ARN}]");
                            var  task= new MessageTask(firstText ?? "Staging", secondText ?? "", "1111-000-33333");

                            string taskJSON = JsonHelper.JsonSerialize<MessageTask>(task);

                            var _SNSJSONMessage = new SNSJSONMessage(taskJSON);

                            var jsonMessage = JsonHelper.JsonSerialize<SNSJSONMessage>(_SNSJSONMessage);

                            PublishRequest publishReq = new PublishRequest()
                            {
                                TargetArn = SNS_TOPIC_ARN,
                                MessageStructure = "json",
                                Message = jsonMessage
                            };

                            publishReq.MessageAttributes.Add("mission", new MessageAttributeValue { StringValue = "ga-aws-task", DataType = "String" });

                            publishReq.MessageAttributes.Add("task", new MessageAttributeValue { StringValue = "staging", DataType = "String" });
                            sb.AppendLine($"Try publish SNSJSON message");
                            var response = snsClient.PublishAsync(publishReq);
                            sb.AppendLine("SNS Message Pushed");

                            s3Log.CreateS3Object("Task", JsonHelper.JsonSerialize2<MessageTask>(task), ref sb);

                            s3Log.CreateS3Object("SNSJSONMessage", JsonHelper.JsonSerialize2<SNSJSONMessage>(_SNSJSONMessage), ref sb);

                            s3Log.CreateS3Object("PublishRequest", JsonHelper.JsonSerialize2<PublishRequest>(publishReq), ref sb);

                            

                            sb.AppendLine($"SNS[{publishReq.TargetArn}]Message [{publishReq.Message}] created, response HttpStatusCode [{response.Result.HttpStatusCode}]");
                            sb.AppendLine($"published SNS message");
                        }
                        break;

                    case CaseLOG:
                        {
                            context.Logger.LogInformation(firstText ?? "No Log Text");

                            sb.AppendLine($"Log Sent");
                        }

                        break;

                    case CaseTempFile:
                        {
                            var path = System.IO.Path.Combine($"/tmp/file.txt");
                            FileInfo fi = new FileInfo(path);

                            File.WriteAllText(path, firstText ?? "No Written Text");
                            sb.AppendLine($"path :[{fi.FullName}], text:[{File.ReadAllText(fi.FullName)}]");
                        }

                        break;

                    case CaseRDS:
                        {
                            sb.AppendLine($"Start LambdaRDS");
                            string? myConnectionString = null;
                            try
                            {
                                myConnectionString = System.Environment.GetEnvironmentVariable(EnvConnStringConst);
                                sb.AppendLine($"Connection String : [{myConnectionString}]");
                                MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();

                                sb.AppendLine($"new MySql.Data.MySqlClient.MySqlConnection()");
                                conn.ConnectionString = myConnectionString;
                                sb.AppendLine($"Try to open connection");
                                conn.Open();

                                sb.AppendLine($"Connection Opened [{myConnectionString}]");
                            }
                            catch (Exception ex)
                            {
                                sb.AppendLine($"Exception: {ex.Message} [{myConnectionString}] [{ex.GetType()}]");
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Exception [{ex.Message}]");
            }

            context.Logger.LogInformation(sb.ToString());
            return sb.ToString();

        }
    }
}