using UnityEngine;
using UnityEngine.UI;

public static class MonoBehaviourExtension
{
    static void CheckInChildError(this MonoBehaviour target, string T) => throw new($"{target.gameObject.name}'s Child need to add {T} in EditorMode");
    public static T GetComponentSafety<T>(this MonoBehaviour target, out T v) where T : Component
    {
        if (!target.TryGetComponent(out T component))
        {
            component = target.gameObject.AddComponent<T>();
        }
        return v = component;
    }
    public static T CheckInChild<T>(this MonoBehaviour target, out T v) where T : Component
    {
        v = target.GetComponentInChildren<T>();
        if (!v)
        {
            CheckInChildError(target, typeof(T).Name);
        }
        return v;
    }
    public static void CheckScreenSpaceCanvas(this MonoBehaviour target, UnityEngine.Camera camera)
    {
        target.GetComponentSafety(out Canvas canvas);
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        target.GetComponentSafety(out GraphicRaycaster ray);
        GetCanvasScaler(target);
    }
    public static void WorldSpaceCanvas(this MonoBehaviour target, UnityEngine.Camera camera)
    {
        target.GetComponentSafety(out Canvas canvas);
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = camera;
        target.GetComponentSafety(out GraphicRaycaster ray);
        GetCanvasScaler(target);
    }
    static void GetCanvasScaler(this MonoBehaviour target)
    {
        target.GetComponentSafety(out CanvasScaler scaler);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new(2560, 1440);
    }
}
