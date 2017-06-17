namespace Meru
{
    public class PrefixValue
    {
        public static bool operator ==(PrefixValue c1, string c2)
        {
            return c1.Value == c2;
        }

        public static bool operator ==(string c1, PrefixValue c2)
        {
            return c1 == c2.Value;
        }

        public static bool operator !=(PrefixValue c1, string c2)
        {
            return c1.Value != c2;
        }

        public static bool operator !=(string c1, PrefixValue c2)
        {
            return c1 != c2.Value;
        }

        public string Value { get; internal set; } = "";

        public PrefixValue(string value)
        {
            Value = value;
        }

        public static PrefixValue Set(string value)
        {
            return new PrefixValue(value);
        }

        public static PrefixValue Mention
        {
            get
            {
                return new PrefixValue("mention");
            }
        }
    }
}