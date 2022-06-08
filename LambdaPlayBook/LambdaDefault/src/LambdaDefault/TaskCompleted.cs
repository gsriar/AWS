using Newtonsoft.Json;

namespace LambdaDefault
{
    public class TaskStatus
    {
        public TaskStatus(string name, string status)
        {
            this.Status = status;
            this.Name = name;
            @default = "Empty Content";
        }
        public string @default
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }

    }
}