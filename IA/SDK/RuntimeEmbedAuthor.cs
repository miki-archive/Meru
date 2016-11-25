using System;
using Discord;
using IA.SDK.Interfaces;

namespace IA.SDK
{
    internal class RuntimeEmbedAuthor : IEmbedAuthor
    {
        private EmbedAuthorBuilder author;

        public RuntimeEmbedAuthor(EmbedAuthorBuilder author)
        {
            this.author = author;
        }

        public string IconUrl
        {
            get
            {
                return author.IconUrl;
            }

            set
            {
                author.IconUrl = value;
            }
        }
        public string Name
        {
            get
            {
                return author.Name;
            }

            set
            {
                author.Name = value;
            }
        }
        public string Url
        {
            get
            {
                return author.Url;
            }

            set
            {
                author.Url = value;
            }
        }

        public EmbedAuthorBuilder ToNativeObject()
        {
            return author;
        }
    }
}