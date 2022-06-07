using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaTempFile
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
            try
            {
                var path = System.IO.Path.Combine("/tmp/file.txt");
                FileInfo fi = new FileInfo(path);

                File.WriteAllText(path, "I write it");
                return ($"path :[{fi.FullName}], text:[{File.ReadAllText(fi.FullName)}]");
            }
            catch (Exception e)
            {
                return $"Exception: {e.Message}";
            }
        }
    }
}