using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordColor = Discord.Color;

namespace EngageTechTask.Discord.Common
{
    public static class Config
    {
        public static string EmbedSpacer { get; set; } = "https://cdn.animeinterlink.com/r/embed_spacer.png";

        public static Colors Colors { get; set; } = new Colors();
    }

    public class Colors
    {
        public DiscordColor Primary { get; set; } = Convert.ToUInt32("0x0099FF", 16);
        public DiscordColor Success { get; set; } = Convert.ToUInt32("0x3091BF", 16);
        public DiscordColor Warning { get; set; } = Convert.ToUInt32("0xfee75c", 16);
        public DiscordColor Error { get; set; } = Convert.ToUInt32("0xed4245", 16);
        public DiscordColor Invisible { get; set; } = Convert.ToUInt32("0x2f3136", 16);
    }
}