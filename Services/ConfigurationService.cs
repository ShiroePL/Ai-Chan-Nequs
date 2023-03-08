using Ai_Chan.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ai_Chan.Services
{
    public class ConfigurationService
    {
        public string token;
        public string prefix;

        public class Serialized
        {
            [JsonPropertyName("token")]
            public string token { get; set; }
            [JsonPropertyName("prefix")]
            public string prefix { get; set; }
        }

        public ConfigurationService()
        {
            if (!AssertConfigFile())
            {
                Console.WriteLine($@"Template config file created in {Directory.GetCurrentDirectory()}\config.json Edit it and rerun ai-chan.");

                return;
            }
            else
            {
                Serialized serialized = JsonSerializer.Deserialize<Serialized>(File.ReadAllText(Directory.GetCurrentDirectory() + @"\config.json"));
                token = serialized.token;
                prefix = serialized.prefix;
            }
        }

        private bool CreateConfigTemplate()
        {
            Serialized template = new Serialized();
            template.token = "REPLACE_WITH_YOUR_BOT_TOKEN";
            template.prefix = "+";

            try
            {
                File.WriteAllText("config.json", JsonSerializer.Serialize<Serialized>(template));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool AssertConfigFile()
        {
            if (!File.Exists(@$"{Directory.GetCurrentDirectory()}\config.json"))
            {
                if (CreateConfigTemplate())
                    return false;
            }
            return true;
        }
    }
}
