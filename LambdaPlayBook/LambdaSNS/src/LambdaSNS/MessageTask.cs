using Newtonsoft.Json;

namespace LambdaSNS
{
    public class MessageTask
    {
        public MessageTask()
        {
            this.Status = String.Empty; ;
            this.Name = String.Empty;
            BatchID = String.Empty;
        }

        public MessageTask(string name, string status, string batchID)
        {
            this.Status = status;
            this.Name = name;
            BatchID = batchID;
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