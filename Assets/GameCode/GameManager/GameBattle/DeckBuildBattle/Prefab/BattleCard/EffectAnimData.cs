using DG.Tweening;
using System;
using UnityEngine;

[Serializable]
public class EffectAnimData
{
    public bool isAoe;
    public Sprite sprite;
    public float duration = 0.5f;
    public Quaternion rotate => RotationUtils.GetRandomSwingRotation();
}
