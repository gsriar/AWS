using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaDefault
{

    public class Function
    {
       

        private const string RDS = "RDS";
        private const string S3Create = "S3-CREATE";
        private const string S3Delete = "S3-DELETE";
        private const string SNSEMAIL = "SNS-EMAIL";
        private const string SNSJSON = "SNS-JSON";
        private const string LOG = "LOG";
        private const string TempFile = "TEMP-FILE";
        private const string BucketNameConst = "BUCKET_NAME";
        private const string TopicARNConst = "SNS_TOPIC_ARN";
        private const string ConnStringConst = "CONNECTION-STRING-CONST";
     
        private string? SNS_TOPIC_ARN;
        
       
        private AmazonSimpleNotificationServiceClient snsClient;

        IAmazonS3 S3Client { get; set; }
        private string? BUCKET_NAME;
        private string? S3_KEY;
        private string? s3content;

        public Function()
        {
            S3Client = new AmazonS3Client();
            BUCKET_NAME = System.Environment.GetEnvironmentVariable(BucketNameConst);
            S3_KEY = DateTime.Now.ToString("yy-MM(MMM)-dd HHmmssff").ToLower() + ".txt";
            s3content = $"The time is {DateTime.Now.ToString("MMM-dd (ddd) HHmmssff").ToLower()}";

            SNS_TOPIC_ARN = System.Environment.GetEnvironmentVariable(TopicARNConst);
           
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
                sb.AppendLine(String.Join(" | ", "Expected Environment Variables", BucketNameConst, TopicARNConst, ConnStringConst));
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
                    case S3Create:
                        {
                            sb.AppendLine($"BUCKET_NAME [{BUCKET_NAME}]");
                            PutObjectRequest request = new PutObjectRequest();
                            request.BucketName = BUCKET_NAME;
                            request.Key = S3_KEY;
                            request.ContentBody = s3content;
                            request.ContentType = "text/plain";
                            sb.AppendLine($"Try Put S3");
                            var response = S3Client.PutObjectAsync(request);
                            sb.AppendLine($"S3 Action Create [{request.BucketName}] Key: [{request.Key}], response HttpStatusCode [{response.Result.HttpStatusCode}], VersionId [{response.Result.VersionId}]");
                            sb.AppendLine($"Created S3 Object");
                        }
                        break;

                    case S3Delete:
                        {
                            sb.AppendLine($"Try delete S3 - DeleteObjectAsync ");
                            var response = S3Client.DeleteObjectAsync(BUCKET_NAME, firstText);

                            sb.AppendLine($"S3 Action Delete [{BUCKET_NAME}] Key: [{firstText}], response HttpStatusCode [{response.Result.HttpStatusCode}], VersionId [{response.Result.VersionId}]");
                            sb.AppendLine($"delete S3 Object , HttpStatusCode [{response.Result.HttpStatusCode}]");
                        }
                        break;

                    case SNSEMAIL:
                        {
                            sb.AppendLine($"SNS_TOPIC_ARN [{SNS_TOPIC_ARN}]");
                            PublishRequest publishReq = new PublishRequest()
                            {
                                TargetArn = SNS_TOPIC_ARN,
                                Subject = $"{(firstText ?? $"No Subject")} [{DateTime.Now.ToString("MMM-dd HH:mm:ss")}]",
                                Message = $"{(secondText ?? "No Email Body Content")} [{DateTime.Now.ToString("MMM-dd HH:mm:ss")}]",
                            };

                            publishReq.MessageAttributes.Add("purpose", new MessageAttributeValue { StringValue = "notificationEmail", DataType = "String"});

                            sb.AppendLine($"Try publish SNSEMAIL message");
                            var response = snsClient.PublishAsync(publishReq);

                            sb.AppendLine($"SNS[{publishReq.TargetArn}]Message [{publishReq.Message}] created, response HttpStatusCode [{response.Result.HttpStatusCode}]");
                            sb.AppendLine($"published SNS message");
                        }
                        break;

                    case SNSJSON:
                        {
                            sb.AppendLine($"SNS_TOPIC_ARN [{SNS_TOPIC_ARN}]");
                            string msg = JsonConvert.SerializeObject(new TaskStatus(firstText ?? "Staging", secondText ?? ""));
                            PublishRequest publishReq = new PublishRequest()
                            {
                                TargetArn = SNS_TOPIC_ARN,
                                MessageStructure = "json",
                                Message = msg
                            };

                            publishReq.MessageAttributes.Add("purpose", new MessageAttributeValue { StringValue = "LambdaTrigger", DataType = "String" });

                            sb.AppendLine($"Try publish SNSJSON message");
                            var response = snsClient.PublishAsync(publishReq);

                            sb.AppendLine($"SNS[{publishReq.TargetArn}]Message [{publishReq.Message}] created, response HttpStatusCode [{response.Result.HttpStatusCode}]");
                            sb.AppendLine($"published SNS message");
                        }
                        break;

                    case LOG:
                        {
                            context.Logger.LogInformation(firstText ?? "No Log Text");

                            sb.AppendLine($"Log Sent");
                        }

                        break;

                    case TempFile:
                        {
                            var path = System.IO.Path.Combine($"/tmp/file.txt");
                            FileInfo fi = new FileInfo(path);

                            File.WriteAllText(path, firstText ?? "No Written Text");
                            sb.AppendLine($"path :[{fi.FullName}], text:[{File.ReadAllText(fi.FullName)}]");
                        }

                        break;

                    case RDS:
                        {
                            sb.AppendLine($"Start LambdaRDS");
                            string? myConnectionString = null;
                            try
                            {
                                myConnectionString = firstText ?? System.Environment.GetEnvironmentVariable(ConnStringConst);
                                sb.AppendLine($"EnvironmentVariable cstr : [{myConnectionString}]");
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