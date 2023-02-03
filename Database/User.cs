using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Database
{
    public class User
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public string[] nicknames { get; set; }
        public int points { get; set; }
        public long exp { get; set; }
        public byte level {get; set; }
    }
}
