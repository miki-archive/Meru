using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK.Builders
{
    public class MessageBuilder
    {
        string message = "";

        public MessageBuilder AppendText(string text, MessageFormatting formatting = MessageFormatting.PLAIN, bool newLine = true, bool endWithSpace = false)
        {
            if (string.IsNullOrWhiteSpace(text)) return this;

            text = ApplyFormatting(text, formatting);

            if (endWithSpace) text += " ";

            message += text;
            if (newLine) NewLine();

            return this;
        }
        public MessageBuilder NewLine()
        {
            message += "\n";
            return this;
        }

        public string Build()
        {
            return message;
        }
        public string BuildWithBlockCode(string language = "markdown")
        {
            return "```" + language + "\n" + message + "\n```";
        }

        private string ApplyFormatting(string text, MessageFormatting formatting)
        {
            switch(formatting)
            {
                case MessageFormatting.BOLD:
                    return "**" + text + "**";

                case MessageFormatting.BOLD_ITALIC:
                    return "**_" + text + "_**";

                case MessageFormatting.BOLD_ITALIC_UNDERLINED:
                    return "__**_" + text + "_**__";

                case MessageFormatting.ITALIC:
                    return "_" + text + "_";

                case MessageFormatting.ITALIC_UNDERLINED:
                    return "___" + text + "___";

                case MessageFormatting.UNDERLINED:
                    return "__" + text + "__";

                case MessageFormatting.CODE:
                    return "`" + text + "`";

                case MessageFormatting.BLOCKCODE:
                    return "```" + text + "```";

                default:
                    return text;
            }
        }
    }

    public enum MessageFormatting
    {
        PLAIN,
        BOLD,
        ITALIC,
        UNDERLINED,
        BOLD_ITALIC,
        BOLD_UNDERLINED,
        ITALIC_UNDERLINED,
        BOLD_ITALIC_UNDERLINED,
        CODE,
        BLOCKCODE
    }
}
