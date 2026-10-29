using System;
using UnityEngine;

[Serializable]
public class MotionData
{
    public Vector2 pos;
    public Quaternion rotate;
    public Vector3 scale;
    public MotionData(Vector2 vector2, Quaternion quaternion, Vector3 scales)
    {
        this.pos = vector2;
        this.rotate = quaternion;
        this.scale = scales;
    }
}