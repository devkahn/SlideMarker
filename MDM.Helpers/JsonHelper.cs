using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MDM.Helpers
{
    public static class JsonHelper
    {
        public static string ToJsonString(object obj)
        {
            string output = string.Empty;

            output = JsonConvert.SerializeObject(obj);

            return output;
        }
        public static T ToObject<T>(string jsonString)
        {
            T output = JsonConvert.DeserializeObject<T>(jsonString);

            return output;
        }
    }
}
