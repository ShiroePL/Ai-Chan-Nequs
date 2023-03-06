using Ai_Chan.Database;
using Discord;
using Discord.WebSocket;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Services
{
    public class DatabaseService
    {
        public void AddUser(SocketGuildUser user)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");

                var discordUser = new User
                {
                    id = user.Id,
                    name = user.Username,
                    nicknames = new string[] { user.Username },
                    points = 0,
                    exp = 0,
                    level = 1
                };

                col.Insert(discordUser);
            }
        }

        public void AddPoint(ulong id)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");
                var result = col.FindOne(x => x.id == id);
                result.points++;
                col.Update(result);
            }
        }

        public void SubtractPoint(ulong id)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");
                var result = col.FindOne(x => x.id == id);
                result.points--;
                col.Update(result);
            }
        }

        public int GetPoints(ulong id)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");
                var result = col.FindOne(x => x.id == id);
                return result.points;
            }
        }

        public void AddNickname(ulong id, string nickname)
        {
            if (nickname == "")
                return;

            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");
                var result = col.FindOne(x => x.id == id);
                
                string[] newNicknames = new string[result.nicknames.Length + 1];
                for(int i = 0; i < newNicknames.Length; i++)
                {
                    if (i > result.nicknames.Length - 1)
                        newNicknames[i] = nickname;
                    else
                        newNicknames[i] = result.nicknames[i];
                }
                result.nicknames = newNicknames;

                col.Update(result);
            }
        }

        public string[] GetNicknames(ulong id)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");
                var result = col.FindOne(x => x.id == id);
                var nicknames = result.nicknames;

                return nicknames;
            }
        }

        public long GetExp(ulong id)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");
                var result = col.FindOne(x => x.id == id);
                return result.exp;
            }
        }

        public bool AddExp(ulong id, int amount)
        {
            bool levelup = false;

            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");
                var result = col.FindOne(x => x.id == id);

                if (result != null)
                {
                    result.exp += amount;
                    if (result.exp >= (long)(result.level * 100))
                    {
                        result.level++;
                        result.exp = 0;
                        levelup = true;
                    }

                    col.Update(result);
                }
            }

            return levelup;
        }

        public string[] GetLevelInfo(ulong id)
        {
            User result;

            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");
                result = col.FindOne(x => x.id == id);
            }

            return new string[] { $"{result.level}", $"{result.exp}/{result.level*100}" };
        }

        public User GetUser(ulong id)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                ILiteCollection<User> col = db.GetCollection<User>("users");
                User? result = col.FindOne(x => x.id == id);

                return result;
            }
        }

        public string GetLeaderboard()
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<User>("users");
                var results = col.Query().ToList().OrderByDescending(x => x.level).ThenByDescending(x => x.exp).ToList();

                string leaderboard = "";
                for(int i = 0; i < 10; i++)
                {
                    if(i == 9)
                        leaderboard += $"{i + 1}. {results[i].name} | Lv. {results[i].level} | Exp. {results[i].exp}";
                    else
                        leaderboard += $"{i + 1}. {results[i].name} | Lv. {results[i].level} | Exp. {results[i].exp}\n";
                }

                return leaderboard;
            }
        }

        public void AddPicture(string type, string _link)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<Picture>(type);

                var picture = new Picture
                {
                    link = _link,
                };

                col.Insert(picture);
            }
        }

        public string GetRandomPicture(string type)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<Picture>(type);
                var results = col.Query().ToList();

                return results[new Random().Next(results.Count)].link;
            }
        }

        public List<Picture> GetPictures(string type)
        {
            using (var db = new LiteDatabase($@"{new FileInfo(Assembly.GetEntryAssembly().Location).Directory}\database.db"))
            {
                var col = db.GetCollection<Picture>(type);
                var results = col.Query().ToList();

                return results;
            }
        }
    }
}
