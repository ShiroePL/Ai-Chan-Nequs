using Ai_Chan.Services;
using Ai_Chan.Utility;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai_Chan.Modules
{
    [Name("whiteneko")]
    [Summary("Beware!")]
    public class WhitenekoModule : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseService _database;
        private readonly DiscordSocketClient _client;
        public WhitenekoModule(DatabaseService database, DiscordSocketClient client)
        {
            _database = database;
            _client = client;
        }

        private string[] words = new string[]
        { "wealth", "degree", "hole", "honorable", "feigned", "hideous", "bless", "nauseating", "maddening", "guttural",
          "alarm", "unusual", "zoo", "selective", "open", "time", "jelly", "amusing", "scale", "unarmed", "husky",
          "mean", "hanging", "rough", "frame", "judicious", "clean", "furniture", "choke", "petite", "lovely", "protest",
          "spiky", "swing", "practise", "silly", "voracious", "dispensable", "recognise", "quill", "straw", "loving",
          "bump", "branch", "questionable", "idea", "cakes", "peaceful", "rapid", "warm", "produce", "accessible",
          "jumpy", "number", "steep", "aloof", "truthful", "crow", "race", "waste", "robin", "overflow", "remain",
          "tiny", "turn", "dependent", "plastic", "regret", "paddle", "improve", "airplane", "cute", "window", "icicle",
          "entertaining", "effect", "available", "brush", "ambitious", "third", "notice", "descriptive", "subtract",
          "kittens", "fearful", "fabulous", "adamant", "nation", "ring", "uttermost", "education", "rhetorical",
          "squeeze", "stop", "cub", "pot", "arrogant", "mailbox", "quaint", "unequaled", "slimy", "learn", "control",
          "box", "stamp", "absent", "refuse", "rustic", "fixed", "burst", "vacuous", "hallowed", "nail", "unable", "day",
          "taste", "flesh", "determined", "income", "wistful", "fog", "canvas", "hour", "gun", "stereotyped", "enormous",
          "aboriginal", "obese", "birds", "disarm", "cracker", "surround", "crack", "expect", "use", "red", "lamp",
          "puzzled", "cruel", "worry", "lick", "sofa", "nonstop", "abrasive", "snotty", "present", "men", "adjoining",
          "lamentable", "sneeze", "step", "kind", "decide", "strong", "meddle", "drain", "color", "seemly", "join",
          "wool", "material", "intelligent", "twist", "fold", "upset", "haunt", "lackadaisical", "decision", "report",
          "flavor", "pass", "sugar", "confess", "feeble", "electric", "insidious", "idiotic", "group", "hammer",
          "rampant", "value", "market", "memory", "roll", "dear", "thunder", "fang", "cool", "purring", "murky",
          "sturdy", "careless", "statement", "laughable", "erratic", "fruit", "small", "stew", "icky", "cross", "toes",
          "bang", "tip", "living", "camera", "empty", "screw", "man", "wall", "desire", "include", "found", "ghost",
          "cushion", "horse", "houses", "act", "berry", "legs", "nest", "luxuriant", "kneel", "muscle", "winter", "pie",
          "tire", "thin", "post", "sponge", "nod", "dusty", "boast", "addicted", "real", "expansion", "rinse", "fear",
          "flat", "sticks", "diligent", "ticket", "form", "challenge", "hissing", "nervous", "depend", "squirrel",
          "temper", "reply", "scarce", "skillful", "basket", "rainy", "equable", "fumbling", "remind", "water", "used",
          "busy", "crush", "fold", "caption", "tendency", "glistening", "object", "pause", "superficial", "request",
          "jog", "bright", "coat", "pull", "whispering", "wash", "bait", "lumber", "unwieldy", "madly", "reflect",
          "undress", "field", "uneven", "measure", "lucky", "cloudy", "cowardly", "trees", "striped", "impolite",
          "punch", "amount", "push", "attack", "acrid", "place", "advice", "print", "utter", "typical", "bone", "lonely",
          "skin", "giant", "cable", "limping", "tour", "back", "crash", "quartz", "harmony", "lethal", "camp", "tug",
          "kiss", "bleach", "late", "bewildered", "spy", "injure", "sloppy", "depressed", "hypnotic", "breezy",
          "apparatus", "rude", "cars", "proud", "high-pitched", "drag", "cream", "comb", "enthusiastic", "seat",
          "chance", "switch", "vigorous", "save", "scare", "labored", "unsuitable", "toys", "rambunctious", "guide",
          "donkey", "reflective", "premium", "join", "illustrious", "safe", "irate", "insurance", "texture", "crowded",
          "stale", "digestion", "vast", "squalid", "sidewalk", "crib", "amuck", "glorious", "simple", "verdant", "bathe",
          "pastoral", "riddle", "mixed", "planes", "plastic", "friends", "sister", "yard", "extra-large", "obtain",
          "chunky", "hop", "cat", "carriage", "insect", "woebegone", "scary", "past", "smart", "extend", "arm", "orange",
          "mature", "reaction", "jail", "muddle", "repulsive", "kaput", "one", "earthy", "head", "nifty", "aunt",
          "governor", "copy", "fairies", "cloth", "arch", "mitten", "stove", "confused", "boy", "terrific", "sack",
          "imported", "weigh", "hobbies", "sack", "geese", "monkey", "guard", "walk", "wash", "company", "thing",
          "cagey", "eggs", "haircut", "military", "abiding", "reduce", "committee", "deafening", "truculent", "wing",
          "destruction", "guarded", "order", "serious", "end", "interest", "handsome", "hot", "tow", "jagged",
          "habitual", "productive", "bright", "gruesome", "flashy", "unbecoming", "mighty", "land", "railway", "elfin",
          "long", "passenger", "buzz", "boil", "rot", "puncture", "shake", "trip", "funny", "airport", "magnificent",
          "straight", "treat", "hesitant", "glue", "disappear", "satisfying", "head", "support", "pretend", "eminent",
          "cemetery", "crate", "face", "delicate", "optimal", "live", "abject", "happen", "chilly", "blue-eyed",
          "subsequent", "range", "bat", "moor", "smiling", "fade", "exotic", "tightfisted", "hair", "stream", "drip",
          "hands", "word", "clammy", "pets", "scratch", "pigs", "gorgeous", "health", "jam", "suffer", "hook", "zonked",
          "hate", "stir", "mind", "bulb", "street", "obnoxious", "school", "shade", "return", "lewd", "weak",
          "evanescent", "point", "reward", "river", "quiet", "flippant", "smash", "delight", "thirsty", "superb",
          "exultant", "abundant", "mouth", "pies", "numerous", "request", "tart", "steer", "shelf", "ruin", "messy",
          "scene", "delirious", "debt", "hospitable", "smoggy", "curved", "form", "rob", "close", "waste", "attract",
          "handsomely", "thundering", "even", "redundant", "pine", "hill", "violet", "knee", "birth", "guide", "sleet",
          "square", "whisper", "previous", "business", "competition", "dysfunctional", "demonic", "hand", "hard-to-find",
          "nutritious", "teeth", "sore", "nimble", "bad", "wave", "incandescent", "sheep", "dead", "detail", "salty",
          "office", "bells", "unit", "teaching", "sign", "low", "dress", "assorted", "curious", "rhyme", "fireman",
          "saw", "hushed", "prick", "transport", "oven", "taste", "wink", "engine", "government", "defiant", "water",
          "gullible", "accurate", "stupid", "ten", "crook", "tacit", "numberless", "knowing", "food", "gratis", "punish",
          "farm", "heavy", "mother", "owe", "lively", "arrange", "look", "rural", "romantic", "curve", "writing", "fast",
          "queen", "tap", "unknown", "compete", "blink", "guarantee", "weight", "hate", "lumpy", "correct", "cough",
          "gigantic", "deceive", "jump", "woozy", "explode", "intend", "screeching", "terrify", "plate", "low",
          "educated", "harbor", "uptight", "important", "keen", "vagabond", "strange", "maniacal", "alluring", "illegal",
          "dull", "fresh", "complex", "holiday", "library", "annoying", "fuzzy", "light", "grubby", "standing", "rate",
          "observant", "careful", "happy", "adaptable", "dolls", "need", "maid", "property", "pushy", "agreement",
          "cynical", "absorbed", "wire", "claim", "noxious", "talk", "cooperative", "balance", "rescue", "moldy",
          "astonishing", "friend", "humorous", "complete", "parcel", "weary", "horrible", "brother", "super", "shiver",
          "wipe", "ignore", "wry", "pointless", "admit", "whip", "sleepy", "hose", "abrupt", "expert", "common", "poke",
          "smell", "thaw", "nut", "start", "discussion", "whirl", "distribution", "possible", "undesirable", "pen",
          "applaud", "start", "profuse", "marked", "flock", "shop", "attend", "charge", "trains", "breakable", "beds",
          "poison", "bow", "structure", "account", "deserve", "ban", "dream", "beam", "soap", "pocket", "well-made",
          "subdued", "sail", "closed", "rule", "new", "venomous", "dogs", "existence", "wandering", "plant", "skirt",
          "rabid", "magic", "uninterested", "communicate", "share", "snobbish", "prickly", "occur", "angry", "rightful",
          "collar", "efficacious", "faulty", "normal", "belief", "placid", "macabre", "fit", "capable", "silent",
          "stitch", "excuse", "birthday", "obey", "juicy", "destroy", "alert", "wide", "exercise", "lunchroom", "hover",
          "fretful", "toothpaste", "graceful", "part", "encouraging", "dark", "gabby", "knot", "ready", "trick", "meat",
          "play", "dirt", "shock", "face", "harass", "pathetic", "bounce", "abhorrent", "relax", "bubble", "trick",
          "reject", "rush", "wrench", "thirsty", "sand", "offend", "sparkle", "string", "horn", "ship", "precious",
          "sneaky", "seal", "concerned", "camp", "apparel", "identify", "bat", "snow", "spring", "agree", "leather",
          "secretary", "futuristic", "voyage", "overt", "wiry", "spurious", "time", "suck", "next", "sweltering", "lean",
          "announce", "shy", "afford", "chickens", "green", "old-fashioned", "pear", "release", "zipper", "complain",
          "chop", "scorch", "callous", "calculator", "cap", "whistle", "sky", "pencil", "lock", "fool", "teeny-tiny",
          "fearless", "action", "symptomatic", "able", "festive", "tax", "smile", "six", "toad", "fill", "board",
          "table", "plug", "tacky", "wary", "trade", "juvenile", "book", "finger", "long", "dad", "zip", "quick",
          "natural", "versed", "program", "divergent", "x-ray", "old", "dock", "tense", "rule", "plain", "cloistered",
          "dusty", "creator", "rejoice", "answer", "tick", "frame", "borrow", "internal", "bumpy", "count", "scissors",
          "disagreeable", "place", "oranges", "fall", "exuberant", "observe", "level", "scandalous", "brake", "fair",
          "mice", "produce", "aspiring", "name", "gamy", "toothsome", "knit", "bashful", "jail", "size", "sink", "help",
          "meaty", "wonder", "needle", "big", "mourn", "disillusioned", "brawny", "view", "promise", "ray", "different",
          "black-and-white", "power", "hum", "laugh", "fantastic", "reach", "infamous", "stupendous", "transport",
          "corn", "deer", "wrong", "chess", "quarrelsome", "money", "surprise", "store", "brass", "bucket", "stay",
          "paint", "admire", "interrupt", "station", "godly", "kitty", "glossy", "damp", "cart", "narrow", "lively",
          "nappy", "powerful", "air", "move", "queue", "miss", "jumbled", "woman", "hysterical", "cheerful",
          "beneficial", "succinct", "mess up", "merciful", "week", "squeak", "innate", "night", "stem", "follow",
          "harmonious", "witty", "wilderness", "rest", "sprout", "salt", "two", "cover", "naive", "coordinated",
          "amused", "befitting", "incompetent", "talk", "noise", "kiana", "omega", "neko", "pyok", "fly" };

        [Command("spam", RunMode = RunMode.Async)]
        [Summary("Spam is beautiful - WhiteNeko 1493\nExample: spam 5 this will send 5 times")]
        public async Task Spam(int j, [Remainder]string text)
        {
            await Context.Message.DeleteAsync();

            for(int i = 0; i < j; i++)
            {
                await ReplyAsync(text);
            }
        }

        [Command("dm", RunMode = RunMode.Async)]
        [Summary("Basically spam but direct\nExample: dm @WhiteNeko this text will send")]
        public async Task DM(SocketGuildUser user, [Remainder]string text)
        {
            await Context.Message.DeleteAsync();
            await user.CreateDMChannelAsync().Result.SendMessageAsync(text);
        }

        [Command("operation", RunMode = RunMode.Async)]
        [Summary("operationdavid memories\nExample: operation @David")]
        public async Task Operation(SocketGuildUser user)
        {
            for (int i = 10; i > 0; i--)
            {
                await ReplyAsync($"Operation {user.Mention} starts in {i}...");
                Thread.Sleep(1000);
            }

            await ReplyAsync($"Bayo {user.Mention}! You have commited war crimes and you are banished to the death!");
            await user.KickAsync();
        }


        [Command("advertise", RunMode = RunMode.Async)]
        [Summary("WhiteNekos advertisement! Careful with usage! Send a message to everyone in dm\nExample: advertise this text will send")]
        public async Task Ad([Remainder]string text)
        {
            foreach(var user in Context.Guild.Users)
            {
                await user.CreateDMChannelAsync().Result.SendMessageAsync(text);
            }
        }

        [Command("lmao", RunMode = RunMode.Async)]
        [Summary("Best command ever created!\nExample: lmao")]
        public async Task Lmao()
        {
            foreach (var user in Context.Guild.Users)
            {
                try
                {
                    await user.ModifyAsync(x =>
                    {
                        x.Nickname = $"{words[new Random().Next(words.Length)]} {words[new Random().Next(words.Length)]}";
                    });
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        [Command("lmaoo", RunMode = RunMode.Async)]
        [Summary("Lmao v2?\nExample: lmaoo")]
        public async Task Lmaoo()
        {
            foreach (var textchannel in Context.Guild.TextChannels)
            {
                try
                {
                    await textchannel.ModifyAsync(x =>
                    {
                        x.Name = $"{words[new Random().Next(words.Length)]} {words[new Random().Next(words.Length)]}";
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        [Command("autobots roll out", RunMode = RunMode.Async)]
        [Summary("ROLL OUT!")]
        public async Task RollOut() => await ReplyAsync(@"https://tenor.com/view/roll-out-optimus-transformers-optimusprime-gif-8983715");
    }
}
