using System;

[Serializable]
public class TextInfo
{
    public string cn;
    public string en;
    public string jp;

    public bool IsEmpty()
    {
        return cn == string.Empty;
    }
}
