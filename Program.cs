using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace AcegikmoDiscordBot
{
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
#pragma warning disable CS0649
    [DataContract]
    internal class ConfigClass
    {
        [DataMember]
        internal string token;

        [DataMember]
        internal ulong server;

        [DataMember]
        internal ulong channel;
    }
#pragma warning restore CS0649
#pragma warning restore CS8618 // Non-nullable field is uninitialized.
#pragma warning restore IDE1006 // Naming Styles

    internal class Program : IDisposable
    {
        public static ulong ASHL = 139525105846976512UL;

        private readonly Log _log;
        public static readonly ConfigClass Config = GetConfig();
        private readonly DiscordSocketClient _client;

        private static ConfigClass GetConfig()
        {
            using var stream = File.OpenRead("config.json");
            var json = new DataContractJsonSerializer(typeof(ConfigClass));
            return (ConfigClass)json.ReadObject(stream);
        }

        private static async Task Main()
        {
            using var program = new Program();
            await program.Run();
        }

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                ExclusiveBulkDelete = true,
            });
            _log = new Log();
            Console.CancelKeyPress += (sender, args) => Dispose();
        }

        private async Task Run()
        {
            _client.Log += a => { Console.WriteLine(a); return Task.CompletedTask; };
            _client.MessageDeleted += new DeleteEcho(_log).MessageDeletedAsync;
            _client.MessageReceived += new EchoCommand().MessageReceivedAsync;
            _client.MessageReceived += new GamesCommand().MessageReceivedAsync;
            _client.MessageReceived += new HelpCommand().MessageReceivedAsync;
            _client.MessageReceived += new PronounCommand().MessageReceivedAsync;
            _client.MessageReceived += new MemberizerCommand(_log).MessageReceivedAsync;
            _client.MessageReceived += new TimingThing(_log).MessageReceivedAsync;
            _client.MessageReceived += _log.MessageReceivedAsync;
            _client.MessageUpdated += _log.MessageUpdatedAsync;

            await _client.LoginAsync(TokenType.Bot, Config.token);
            await _client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1);
        }

        public void Dispose()
        {
            _client.Dispose();
            _log.Dispose();
        }
    }
}
