using System.Net;
using Discord.Commands;
using Shared.MongoDB;
using DiscordAttachment = Discord.Attachment;
using ChoETL;
using System.Text;
using Discord;
using EngageTechTask.Fetch;

namespace EngageTechTask.Discord.Modules.Text;

public class FileHandler : ModuleBase<ShardedCommandContext>
{
    private readonly CommandService _service;
    private DBClient _dbClient;

    public FileHandler(CommandService service)
    {
        _service = service;
    }

    [Command("Upload")]
    [Summary("Upload a CSV or JSON file")]
    public async Task LatencyAsync()
    {
        int bytesToRead = 8192;

        byte[] buffer = new byte[bytesToRead];

        var attachments = Context.Message.Attachments;

        string file = attachments.ElementAt(0).Filename;
        string url = attachments.ElementAt(0).Url;

        var req = WebRequest.Create(url);
        var res = await req.GetResponseAsync();

        if (res.ContentLength < 1)
            return;

        using (Stream output = File.OpenWrite(file))
        using (Stream input = res.GetResponseStream())
        {
            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        Console.WriteLine("Download successful.");

        await ProcessFile(attachments.ElementAt(0));
        // Place the contents as a message because the method said it should.
        await ReplyAsync("Processed file!\n\n");
    }

    public async Task ProcessFile(DiscordAttachment file)
    {
        _dbClient = new DBClient("EngageBot", Environment.GetEnvironmentVariable("ENGAGEBOTCONNECTIONSTRING"));
        StringBuilder jsonStr = new StringBuilder();
        var fs = new FetchService(_dbClient);

        if (file.ContentType.Split(';')[0].ToString() == "text/csv")
        {
            using (StreamReader sr = new StreamReader(file.Filename))
            using (var p = ChoCSVReader.LoadText(await sr.ReadToEndAsync())
                .WithFirstLineHeader()
                .IgnoreEmptyLine()
                .IgnoreField("users/manager_ref")
                .Configure(cfg => cfg.NullValue = "")
                .NestedKeySeparator('/')
                
            )
            {
                using (var w = new ChoJSONWriter(jsonStr))
                    w.Write(p);
            }

            
        }
        else if (file.ContentType.Split(';')[0].ToString() == "application/json")
        {
            using (StreamReader sr = new StreamReader(file.Filename))
            {
                jsonStr.Append(await sr.ReadToEndAsync());
            }

            var msgChannel = Context.Guild.Channels.First(chnl => chnl.Name == "logs") as IMessageChannel;

            var synced = await fs.SyncUsers(jsonStr.ToString(), msgChannel);

            File.Delete(file.Filename);
        }
        else
        {
            File.Delete(file.Filename);
            var channel = Context.Client.PrivateChannels.First(x => x.Name.ToLower() == "logs") as IMessageChannel;
            if (channel == null)
            {
                return;
            }
            await ReplyAsync("Failed, plese check logs.");
            await channel.SendMessageAsync($"{file.Filename} has an incorrect file type. Please provide CSV or JSON.");
        }
    } 
}