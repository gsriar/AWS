using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedFucntions
{
    public static class JsonHelper
    {
        public static string JsonSerialize<T>(T obj)
        {
          return  JsonConvert.SerializeObject(obj);
        }

        public static T? JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string JsonSerialize2<T>(T? obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            JObject jo = JObject.Parse(json);
            return jo.ToString();
        }

        
    }
}
