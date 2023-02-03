using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Database
{
    public class GuildSettings
    {
        public ulong guildID { get; set; }
        public string prefix { get; set; }
        public ulong welcomeChannelID { get; set; }
    }
}
