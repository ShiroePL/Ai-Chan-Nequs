using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Services
{
    public class EventsService
    {
        private readonly DiscordSocketClient _client;
        private readonly DatabaseService _database;

        public EventsService(DiscordSocketClient client, DatabaseService database)
        {
            _client = client;
            _database = database;

            _client.GuildMemberUpdated += _client_GuildMemberUpdated;
            _client.UserJoined += _client_UserJoined;
            _client.Ready += _client_Ready;
        }

        private Task _client_Ready()
        {

            if (!File.Exists($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\data\database.db"))
            {
                foreach (var guild in _client.Guilds)
                {
                    foreach (var user in guild.Users)
                    {
                        _database.AddUser(user);
                    }
                }

                Console.WriteLine("Database created!");

                return Task.CompletedTask;
            }

            Console.WriteLine("Database exists!");

            return Task.CompletedTask;
        }

        private Task _client_UserJoined(SocketGuildUser arg)
        {
            arg.Guild.GetTextChannel(1034897925203054683).SendMessageAsync($"Welcome to the BakaCats {arg.Username}! (｡◕‿‿◕｡)");

            foreach (var guild in _client.Guilds)
            {
                foreach (var user in guild.Users)
                {
                    if (_database.GetUser(user.Id) != null)
                    {
                        _database.AddUser(user);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task _client_GuildMemberUpdated(Cacheable<SocketGuildUser, ulong> userBefore, SocketGuildUser userAfter)
        {
            if (userBefore.Value.Nickname != userAfter.Nickname)
            {
                _database.AddNickname(userAfter.Id, userAfter.Nickname);
            }

            return Task.CompletedTask;
        }
    }
}
