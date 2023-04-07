using Ai_Chan.Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Ai_Chan.Services
{
    public class GamblingService
    {
        private readonly DiscordSocketClient _client;
        private readonly DatabaseService _database;

        public bool joinable = true;

        public GamblingService(DiscordSocketClient client, DatabaseService database)
        {
            _client = client;
            _database = database;
        }

        public async Task RussianGame(List<IUser> _users, SocketCommandContext _context, int _totalexp)
        {
            string[] fearKaomojis = 
                {
                "＼(〇_ｏ)／",
                "(;;;*_*)",
                "〜(＞＜)〜",
                "(ﾉ_ヽ)",
                "Σ(°△°|||)︴",
                "(″ロ゛)",
                "(ノωヽ)",
                "(/ω＼)"
            };

            string[] painKaomojis =
            {
                "~(>_<~)",
                "☆⌒(> _ <)",
                "☆⌒(>。<)",
                "(☆_@)",
                "(×_×)",
                "٩(× ×)۶",
                "(×﹏×)"
            };

            string[] joyKaomojis =
            {
                "(* ^ ω ^)",
                "(((o(°▽°)o)))",
                "(✯◡✯)",
                "o(>ω <)o",
                "(⁀ᗢ⁀)",
                "ヽ(o＾▽＾o)ノ",
                "(￣ω￣)",
                "(⌒▽⌒)☆"
            };

            joinable = false;
            int i = 0;

            while(_users.Count > 1)
            {
                await _context.Channel.SendMessageAsync($"{_users[i].Username} presses the revolver to their head and slowly squeezes the trigger...{fearKaomojis[new Random().Next(fearKaomojis.Length - 1)]}\n");
                Thread.Sleep(3500);

                if (new Random().Next(3) == 1)
                {
                    await _context.Channel.SendMessageAsync($"BOOOM! {_users[i].Mention} goes down! {painKaomojis[new Random().Next(fearKaomojis.Length - 1)]}");
                    _users.Remove(_users[i]);
                    i--;
                }
                else
                {
                    await _context.Channel.SendMessageAsync($"CLICK! {_users[i].Mention} passes the gun along.");
                }

                i++;

                if (i == _users.Count)
                    i = 0;
            }

            if (_users.First().Id != 452541322667229194)
            {
                await _context.Channel.SendMessageAsync($"{_users.First().Mention} Congratulations! You won {_totalexp} exp! {painKaomojis[new Random().Next(fearKaomojis.Length - 1)]}");
            }
            else
            {
                await _context.Channel.SendMessageAsync($"(￢‿￢ ) Easy win, free exp");
            }

            if (_database.AddExp(_users.First().Id, _totalexp))
            {

                if (_users.First().Id != 452541322667229194)
                {
                    await _context.Channel.SendMessageAsync($"Congratulations {_users.First().Mention}! You leveled up but everyone's DED so you can't even flex! OmO");
                }
                else
                {
                    await _context.Channel.SendMessageAsync($"(￢‿￢ ) Too easy to level up on you noobs!");
                }
            }

            joinable = true;
        }
    }
}
