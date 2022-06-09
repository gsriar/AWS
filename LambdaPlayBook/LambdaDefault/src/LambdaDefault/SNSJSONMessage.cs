namespace LambdaDefault
{
    public class SNSJSONMessage
    {
        public SNSJSONMessage(string @default)
        {
            this.@default = @default;
        }

        public string @default
        {
            get;
            set;
        }

    }
}