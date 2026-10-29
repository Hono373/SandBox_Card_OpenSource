using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class CameraObj : MonoBehaviour
{
    public new UnityEngine.Camera camera;
    public Action OnUpdate;
    public void Update()
    {
        OnUpdate?.Invoke();
    }
    public void Init()
    {
        camera = this.GetComponentSafety(out camera);
    }
}