using System;
using UnityEngine;

[Serializable]
public class ArrangeData
{
    public Vector2[] points;
    public Quaternion[] rotates;
    public Vector3 scales;
    public ArrangeData(Vector2[] points, Quaternion[] rotations, Vector3 scales)
    {
        this.points = points;
        this.rotates = rotations;
        this.scales = scales;
    }
}
