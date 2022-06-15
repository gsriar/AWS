using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedFucntions
{
    public static class Constants
    {

        private const string EnvS3MainBucket = "S3_MAIN_BUCKET";
        private const string EnvTopicARNConst = "SNS_TOPIC_ARN";
        private const string EnvConnStringConst = "CONNECTION_STRING";
        private const string EnvLogBucketNameConst = "S3_log_BUCKET";


        public static string S3MainBucket
        {
            get
            {
                return getVar(EnvS3MainBucket);
            }
        }

        public static string S3LogBucket
        {
            get
            {
                return getVar(EnvLogBucketNameConst);
            }
        }

        public static string SnsTopicARN
        {
            get
            {
                return getVar(EnvTopicARNConst);
            }
        }

        public static string ConnectionString
        {
            get
            {
                return getVar(EnvConnStringConst);
            }
        }

        static string getVar(string key)
        {
            return System.Environment.GetEnvironmentVariable(key)??string.Empty;
        }
    }
}
