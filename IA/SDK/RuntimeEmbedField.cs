using System;
using IA.SDK.Interfaces;
using Discord;

namespace IA.SDK
{
    public class RuntimeEmbedField : IEmbedField
    {
        EmbedField field;

        public RuntimeEmbedField(EmbedField f)
        {
            field = f;
        }

        public bool IsInline
        {
            get
            {
                return field.Inline;
            }

            set
            {
                field.Inline = value;
            }
        }

        public string Name
        {
            get
            {
                return field.Name;
            }

            set
            {
                field.Name = value;
            }
        }

        public string Value
        {
            get
            {
                return field.Value;
            }

            set
            {
                field.Value = value;
            }
        }
    }
}