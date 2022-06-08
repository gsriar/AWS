using Newtonsoft.Json;

namespace LambdaDefault
{
    public class EmailMessage
    {
        public EmailMessage(string state)
        {
            var msg= new message() { Completed = state };

            TaskComlpleted = state;

            //this.EmailMessage = JsonConvert.SerializeObject(msg);

            @default = "Default";
        }
        public string @default
        {
            get;
            set;
        }

        public string TaskComlpleted
        {
            get;
            set;
        }

      

        public class message
        {
            public string? Completed
            {
                get;
                set;
            }
        }
    }
}