using Newtonsoft.Json;

namespace LambdaDefault
{
    public class TaskStatus
    {
        public TaskStatus(string name, string status, string batchID)
        {
            this.Status = status;
            this.Name = name;
            @default = $"{this.Name} | {this.Status}";
            BatchID = batchID;
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

        public string BatchID
        {
            get;
            set;
        }

    }
}