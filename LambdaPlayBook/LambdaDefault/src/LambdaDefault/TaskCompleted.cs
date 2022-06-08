using Newtonsoft.Json;

namespace LambdaDefault
{
    public class TaskCompleted
    {
        public TaskCompleted(string state)
        {
            StageComlpleted = state;
            @default = "Default";
        }
        public string @default
        {
            get;
            set;
        }

        public string StageComlpleted
        {
            get;
            set;
        }

    }
}