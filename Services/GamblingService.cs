using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Services
{
    public class GamblingService
    {
        private readonly DiscordSocketClient _client;
        private readonly DatabaseService _database;

        private bool joinable = true;
        private bool started = false;

        public GamblingService(DiscordSocketClient client, DatabaseService database)
        {
            _client = client;
            _database = database;
        }

        public async Task LetJoining()  => joinable = true;

        public async Task StopJoining() => joinable = false;

        public bool Joinable() => joinable;

        public bool Started() => started;

        /*
        public async Task Russian(ulong userID)
        {
            Console.WriteLine($"{userID} has joined!");
        } 

        public async Task Slots(ulong userID)
        {

        }
        */
    }
}
