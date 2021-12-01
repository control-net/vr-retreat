using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VrRetreat.Core;

namespace VrRetreat.Infrastructure
{
    public class JsonConfiguration : IConfiguration
    {
        public static JsonConfiguration FromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var result = JsonConvert.DeserializeObject<JsonConfiguration>(json);

            if (result is null)
                throw new Exception("JSON file resulted in a null object.");

            return result;
        }

        public string VrChatUsername { get; set; }

        public string VrChatPassword { get; set; }
    }
}
