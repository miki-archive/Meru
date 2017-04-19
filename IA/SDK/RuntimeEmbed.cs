using Discord;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;

namespace IA.SDK
{
    public class RuntimeEmbed : IDiscordEmbed, IProxy<EmbedBuilder>, IQuery<RuntimeEmbed>
    {
        public EmbedBuilder embed;

        public List<IEmbedField> fields = new List<IEmbedField>();

        public RuntimeEmbed(EmbedBuilder e)
        {
            embed = e;
        }

        public IEmbedAuthor Author
        {
            get
            {
                return new RuntimeEmbedAuthor(embed.Author);
            }
            set
            {
                embed.Author.Name = value.Name;
                embed.Author.IconUrl = value.IconUrl;
                embed.Author.Url = value.Url;
            }
        }

        public Color Color
        {
            get
            {
                return new Color(embed.Color.Value.R, embed.Color.Value.G, embed.Color.Value.B);
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

        public string ThumbnailUrl
        {
            get => embed.ThumbnailUrl;
            set => embed.ThumbnailUrl = value;
        }

        public IDiscordEmbed AddField(Action<IEmbedField> field)
        {
            IEmbedField f = new RuntimeEmbedField("", "");

            field.Invoke(f);

            embed.AddField(x =>
            {
                x.Name = f.Name;
                x.Value = f.Value;
                x.IsInline = f.IsInline;
            });

            return this;
        }
        public IDiscordEmbed AddField(IEmbedField field)
        {
            embed.AddField(x =>
            {
                x.Name = field.Name;
                x.Value = field.Value;
                x.IsInline = field.IsInline;
            });

            return this;
        }

        public IEmbedAuthor CreateAuthor()
        {
            embed.Author = new EmbedAuthorBuilder();
            return Author;
        }

        public void CreateFooter()
        {
            embed.Footer = new EmbedFooterBuilder();
        }

        public RuntimeEmbed Query(string embed)
        {
            string[] cutEmbed = embed.Slice();

            foreach (string x in cutEmbed)
            {
                switch (x.Split('{')[0].ToLower().Trim(' '))
                {
                    case "title":
                        {
                            Title = x.Peel();
                        }
                        break;
                    case "description":
                    case "desc":
                        {
                            Description = x.Peel();
                        } break;
                    case "url":
                        {
                            Url = x.Peel();
                        } break;
                    case "imageurl":
                        {
                            ImageUrl = x.Peel();
                        } break;
                    case "color":
                    case "c":
                        {
                            string[] colorSplit = x.Peel().Split(',');
                            Color = new Color(float.Parse(colorSplit[0]), float.Parse(colorSplit[1]), float.Parse(colorSplit[2]));
                        } break;
                    case "author":
                        {
                            Author = (Author as IQuery<RuntimeEmbedAuthor>).Query(x.Peel());
                        } break;
                    case "footer":
                        {
                            Footer = (Footer as IQuery<RuntimeEmbedFooter>).Query(x.Peel());
                        } break;
                    case "field":
                        {
                            RuntimeEmbedField em = new RuntimeEmbedField();
                            AddField((em as IQuery<RuntimeEmbedField>).Query(x.Peel()));
                        } break;
                }
            }

            return this;
        }

        public EmbedBuilder ToNativeObject()
        {
            return embed;
        }
    }
}