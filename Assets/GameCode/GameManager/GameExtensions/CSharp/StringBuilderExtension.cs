using System.Text;

public static class StringBuilderExtension
{
    static StringBuilder builder;
    public static string Combine(this StringBuilder builder, params string[] array)
    {
        builder.Clear();
        foreach (string s in array)
        {
            builder.Append(s);
        }
        return builder.ToString();
    }
}
