using System;
using IA.SDK.Interfaces;
using Discord;

namespace IA.SDK
{
    public class RuntimeEmbedField : IEmbedField
    {
        EmbedFieldBuilder field;

        public RuntimeEmbedField(IEmbedField f)
        {
            field = new EmbedFieldBuilder();
            field.Name = f.Name;
            field.Value = f.Value;
            field.IsInline = f.IsInline;
        }
        public RuntimeEmbedField(string Name, string Value, bool Isinline = false)
        {
            field = new EmbedFieldBuilder();
            field.Name = Name;
            field.Value = Value;
            field.IsInline = IsInline;
        }

        public bool IsInline
        {
            get
            {
                return field.IsInline;
            }

            set
            {
                field.IsInline = value;
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