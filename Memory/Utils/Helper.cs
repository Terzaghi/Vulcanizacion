namespace Memory.Common.Utils
{
    public static class Helper
    {
        public static string ToKey(this TagValue tagValue)
        {
            string key = null;

            if (tagValue != null)
            {
                key = string.Format("{0}.{1}", tagValue.Id_Prensa, (int)tagValue.Type);
            }

            return key;
        }

        public static string ToKey(int Id_Prensa, TagType Type)
        {
            return string.Format("{0}.{1}", Id_Prensa, (int)Type);
        }
    }
}
