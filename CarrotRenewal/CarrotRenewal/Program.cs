using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CarrotBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Runbot().GetAwaiter().GetResult();

        public DiscordSocketClient client;//websocket메인
        private CommandService commands;//Discord 명령 프레임워크를 제공
        private IServiceProvider services; //서비스 객체, 즉 다른 객체에 사용자 지정 지원을 제공하는 객체의 리트리빙 메커니즘을 규정한다.
       
        public async Task Runbot()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();
            ;
            string botpre = "NDc3MDE4MzUyNzIwODA1ODk4.D0kz0A.d - _iIBZNA3B9y7TkI5ll8_OSPgI";
 
            client.Log += Log;
            client.UserJoined += AnnounceUser;

            await RegisterCommands();

            await client.LoginAsync(TokenType.Bot, botpre);

            await client.StartAsync();

            await Task.Delay(-1);
            
        }
        private async Task AnnounceUser(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.DefaultChannel;
            await channel.SendMessageAsync($"Welcome, {user.Mention}");
        }
        private Task Log(LogMessage log)
        {
            Console.WriteLine(log);

            return Task.CompletedTask;
        }

        public async Task RegisterCommands()
            {
            client.MessageReceived += Handlecommand;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task Handlecommand(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argpos=0;
             
            if (message.HasStringPrefix("/", ref argpos)||message.HasMentionPrefix(client.CurrentUser, ref argpos))
            {
                var context = new SocketCommandContext(client, message);

                var resulty = await commands.ExecuteAsync(context, argpos, services);

                if (!resulty.IsSuccess)
                    Console.WriteLine(resulty.ErrorReason);
            } 
        }
    }
}
