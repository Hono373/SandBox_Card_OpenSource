using Spine.Unity;
using System;
using UnityEngine;

/// <summary>
/// 单位图形信息
/// </summary>
[Serializable]
public class BattleUnitGraphicInfo
{
    public SkeletonDataAsset SkeletonDataAsset;
    public Vector2 actionPos;
    public int hpBarWidth;
}
