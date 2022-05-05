using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace CarrotBot
{
    
    [Group("ppng")]
    public class Ping:ModuleBase<SocketCommandContext>
    {
        CommandService commands;
        [Command]
        public async Task Defaltping()
        {
            await ReplyAsync("pong!");
        }
        [Command("user")]
        public async Task Pinguser(SocketGuildUser user)
        {
            await Context.Channel.SendFileAsync(@"/IMGCache/C1.jpg");
            
        }
    }
    public class MainCarrot:ModuleBase<SocketCommandContext>
    {
        [Command("siris")]
        public async Task Pinguser()
        {
            await Context.Channel.SendFileAsync(@"/IMGCache/C1.jpg");

        }
        [Command("폴")]
        public async Task Pinguseraa()
        {
            await Context.Channel.SendFileAsync(@"/IMGCache/Paul.jpg");

        }
        [Command("ping")]
        public async Task pingAs()
        {
            await ReplyAsync("Hello World!");
        }
        [Command("basic")]
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
            builder2.AddField("Main", "/ping", true)
                .AddField("Field2", "Test2")
                .AddField("Field3", "Test3");

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
        public async Task Pluginh([Remainder]string stuff)
        {
            await ReplyAsync(stuff);
        }

        [Command("basic"), RequireOwner]
        public async Task pingasd()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("어케했노 시발려ㄴ아")
                .WithDescription("thisis asdasd")
                .WithColor(Color.Blue);

            await ReplyAsync("", true, builder.Build());
        }
        [Command("나무")]
        public async Task treewiki([Remainder]string full)
        {
            string uoe = "https://namu.wiki/w/" + full;
            Uri ui = new Uri(uoe);
            string Textmade = full+"의 검색결과 입니다";
            EmbedBuilder bild = new EmbedBuilder();
            bild.AddField(Textmade, ui);
            await ReplyAsync("", false, bild.Build());
        }
    }

}
