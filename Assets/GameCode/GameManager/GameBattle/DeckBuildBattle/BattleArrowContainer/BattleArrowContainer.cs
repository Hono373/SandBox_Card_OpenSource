using System;
using System.Numerics;
using UnityEngine;

public class BattleArrowContainer : MonoBehaviour
{
    [HideInInspector] public BattleArrowLine arrowLine;
    public void Init(DeckBuildBattle ddb)
    {
        this.CheckInChild(out arrowLine).Init();
    }

    internal void PrepareForDrag(UnityEngine.Vector2 pos)
    {
        arrowLine.Drag(pos, pos);
        arrowLine.Alpha(true);
    }
}
