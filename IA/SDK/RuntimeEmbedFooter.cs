using Discord;
using IA.SDK.Interfaces;

namespace IA.SDK
{
    public class RuntimeEmbedFooter : IEmbedFooter, IProxy<EmbedFooterBuilder>
    {
        private EmbedFooterBuilder footer;

        public RuntimeEmbedFooter(EmbedFooterBuilder footer)
        {
            this.footer = footer;
        }

        public string IconUrl
        {
            get
            {
                return footer.IconUrl;
            }

            set
            {
                footer.IconUrl = value;
            }
        }

        public string Text
        {
            get
            {
                return footer.Text;
            }

            set
            {
                footer.Text = value;
            }
        }

        public EmbedFooterBuilder ToNativeObject()
        {
            return footer;
        }
    }
}