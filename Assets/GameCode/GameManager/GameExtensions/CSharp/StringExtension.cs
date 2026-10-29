public static class StringExtension
{
    public static string TrimPrefix(this string source, string prefix)
    {
        if (source.StartsWith(prefix))
        {
            return source.Substring(prefix.Length);
        }
        return source;
    }
}
