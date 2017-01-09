using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Discord;

namespace IA.SDK
{
    public class RuntimeEmbedBuilder : IDiscordEmbed, IProxy<EmbedBuilder>
    {
        public EmbedBuilder embed;

        public List<IEmbedField> fields = new List<IEmbedField>();

        public RuntimeEmbedBuilder(EmbedBuilder e)
        {
            embed = e;
        }

        public IEmbedAuthor Author
        {
            get
            {
                return new RuntimeEmbedAuthor(embed.Author);
            }
        }

        public System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb(embed.Color.Value.R, embed.Color.Value.G, embed.Color.Value.B);
            }

            set
            {
                embed.Color = new Discord.Color(value.R, value.G, value.B);
            }
        }

        public string Description
        {
            get
            {
                return embed.Description;
            }

            set
            {
                embed.Description = value;
            }
        }

        public IEmbedFooter Footer
        {
            get
            {
                return new RuntimeEmbedFooter(embed.Footer);
            }

            set
            {
                embed.Footer = new RuntimeEmbedFooter(embed.Footer).ToNativeObject();
            }
        }

        public string ImageUrl
        {
            get
            {
                return embed.ImageUrl;
            }

            set
            {
                embed.ImageUrl = value;
            }
        }

        public string Title
        {
            get
            {
                return embed.Title;
            }

            set
            {
                embed.Title = value;
            }
        }

        public string Url
        {
            get
            {
                return embed.Url;
            }

            set
            {
                embed.Url = value;
            }
        }

        public IDiscordEmbed AddField(Action<IEmbedField> field)
        {
            IEmbedField f = new RuntimeEmbedField("", "");

            field.Invoke(f);
            fields.Add(f);
            return this;
        }

        public EmbedBuilder ToNativeObject()
        {
            return embed;
        }
    }
}
