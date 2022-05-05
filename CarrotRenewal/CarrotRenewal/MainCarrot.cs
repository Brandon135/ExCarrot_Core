using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.IO;
using VideoLibrary;
using MediaToolkit.Model;
using MediaToolkit;
using System.Diagnostics;
using System.Threading;

namespace CarrotBot
{

    [Group("ping")]
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task Defaltping()
        {
            await ReplyAsync("pong!");
        }
        [Command("user")]
        public async Task Pinguser(SocketGuildUser user)
        {
            await ReplyAsync($"poing! {user.Mention}");
        }
        public async Task test()
        {
            await ReplyAsync("lololol");

        }
    }
    public class TextGO : ModuleBase<SocketCommandContext>
    {

        [Command("request")]
        public async Task sendfile()
        {
            string[] item = Directory.GetFiles(@"C:\Users\q\Desktop\Discorddata");
            string textlist = System.IO.File.ReadAllText(item[item.Length - 1]);
            Console.WriteLine(textlist);
            await ReplyAsync(textlist);
        }
        [Command("save")]
        public async Task savefile()
        {
            string[] item = Directory.GetFiles(@"C:\Users\q\Desktop\Discorddata");

            await Context.Channel.SendFileAsync(item[item.Length - 1]);
        }
    }
    public class MainCarrot : ModuleBase<SocketCommandContext>
    {
        private Thread Minecraft;

        Random r = new Random();

        [Command("data")]
        public async Task RandomMap()
        {
            string[] item = Directory.GetFiles(@"S:\OsuMapple");

            await Context.Channel.SendFileAsync(item[r.Next(0, item.Length)]);
        }
        [Command("hi")]
        public async Task hiuser()
        {
            await ReplyAsync($"Hello {Context.User.Mention}");
        }
        [Command("how?")]
        public async Task howAs()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("어케했노 시발려ㄴ아")
                .WithDescription("thisis asdasd")
                .WithColor(Color.Blue);

            await ReplyAsync("", true, builder.Build());
        }
        [Command("help")]
        public async Task teAs()
        {
            EmbedBuilder builder2 = new EmbedBuilder();
            builder2.AddField("인사하기", "/hi", true)
                .AddField("베이직", "/basic")
                .AddField("유튜브 음원추출", "/y")
                .AddField("contx", "/contx");

            await ReplyAsync("", false, builder2.Build());
        }
        [Command("contx")]
        public async Task contx()
        {
            await ReplyAsync($"{Context.Client.CurrentUser}||{Context.User.Mention} sent {Context.Message.Content} in {Context.Guild.Name}!");
            //                 --------- Bot Name --------   ======호출유저=======         ======호출키워드=====      =======Room Name=======
        }
        [Command("kick"), RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickUser(SocketGuildUser userName)
        {
            var user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Role");
            if (!userName.Roles.Contains(role))
            {
                // Do Stuff
                if (user.GuildPermissions.KickMembers)
                {
                    await userName.KickAsync();
                }
            }
        }

        [Command("pz")]
        public async Task Pingasnty()
        {
            await ReplyAsync($"{Context.User.Mention} is a 피젝");
        }

        [Command("say")]
        public async Task Pluginh([Remainder] string stuff)
        {
            await ReplyAsync(stuff);
        }

        [Command("y")]
        public async Task YoutubeDown([Remainder] string stuff)
        {
            var task1 = Task.Run(async () =>
            {
                try
                {
                    await ReplyAsync("처리중");
                    ResetFolder();
                    YoutubeMp3(stuff);
                    string[] root = Directory.GetFiles(@"C:\Users\fun67\Desktop\Youtubedata\");
                    await Context.Channel.SendFileAsync(root[1]);
                }
                catch (Exception e)
                {
                    await ReplyAsync("error :" + e.Message);
                }
            });
        }

        [Command("basic"), RequireOwner]
        public async Task pingasd()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("어케했노 시발려ㄴ아")
                .WithDescription("this is Basic")
                .WithColor(Color.Blue);

            await ReplyAsync("", true, builder.Build());
        }
        [Command("나무")]
        public async Task treewiki([Remainder] string full)
        {
            string uoe = "https://namu.wiki/w/" + full;
            Uri ui = new Uri(uoe);
            string Textmade = full + "의 검색결과 입니다";
            EmbedBuilder bild = new EmbedBuilder();
            bild.AddField(Textmade, ui);
            await ReplyAsync("", false, bild.Build());
        }
        
        private void YoutubeMp3(string url)
        {
            
            {
                var source = @"C:\Users\fun67\Desktop\Youtubedata\";
                var youtube = YouTube.Default;
                var vid = youtube.GetVideo(url);
                File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

                var inputFile = new MediaFile { Filename = source + vid.FullName };
                string FinURL = source + vid.FullName;
                FinURL =FinURL.Replace("- YouTube.mp4", "");
                Debug.WriteLine(FinURL);
                var outputFile = new MediaFile { Filename = $"{FinURL}.mp3" };

                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);

                    engine.Convert(inputFile, outputFile);
                }
            }
           
        }
        private void ResetFolder()
        {
            var dir = @"C:\Users\fun67\Desktop\Youtubedata\";
            string[] files = Directory.GetFiles(dir);
            foreach (string s in files)
            {
                string filename = s;
                File.Delete(filename);
            }
        }
        private string IfNameYoutube(string url)
        {
            string Title;
            while (true)
            {

                YouTube ytb = YouTube.Default; //starting point for YouTube actions
                var vid = ytb.GetVideo(url); // gets a Video object with info about the video

                string ttl = vid.Title;//get video Title
                if (ttl != "youtube")
                {
                   Title =  ttl;
                    break;
                }
            }
            return Title;
        }
    }
}
