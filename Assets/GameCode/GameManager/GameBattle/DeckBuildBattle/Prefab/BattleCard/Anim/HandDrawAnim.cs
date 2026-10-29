using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class HandDrawAnim
{
    public Vector2 startPos = new(1680, 0);
    public Vector2 displayPos = new(760, 0);
    public float createToDisplaySpd = 0.25f;
    public float displayTime = 0.25f;
    public float displayScale = 1;
    public float displayToHandSpd = 0.25f;
    public MotionData data => new(displayPos, Quaternion.identity, displayScale * Vector3.one);
}