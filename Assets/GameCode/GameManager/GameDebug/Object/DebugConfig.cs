using UnityEngine;
[CreateAssetMenu(fileName = nameof(DebugConfig))]
public class DebugConfig : ScriptableObject
{
    public bool isActive = true;
    public bool common = true;
    public enum Type
    {
        Common,
    }
    public void Log(string message, Type type, bool needFrameCount = false)
    {
#if UNITY_EDITOR
        var frameCount = string.Empty;
        if (!isActive) return;
        switch (type)
        {
            case Type.Common:
                if (!common) return;
                break;
        }

        if (needFrameCount)
            frameCount = $"帧[{Time.frameCount}]";

        UnityEngine.Debug.Log($"{frameCount}:{message}");
#endif
    }
}