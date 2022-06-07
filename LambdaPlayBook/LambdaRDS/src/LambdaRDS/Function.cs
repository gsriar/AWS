using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaRDS
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            string? myConnectionString = null;
            try
            {
                MySql.Data.MySqlClient.MySqlConnection conn;
                myConnectionString = System.Environment.GetEnvironmentVariable("cstr");
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
                return $"Connection Opened [{myConnectionString}]";
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message} [{myConnectionString}] [{ex.GetType()}]";
            }
        }
    }
}
