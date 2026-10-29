using UnityEngine;

public class NoteInfo : BaseInfo
{
    public string describe = string.Empty;
    public NoteInfo Init(StoryInfo info, Vector2 v)
    {
        Set(info, v);
        return this;
    }
}