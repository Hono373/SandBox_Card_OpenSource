using System;
using UnityEngine;
[Serializable]
public class BattleCardSelectInfo
{
    [SerializeReference] public ISelectCheck selectCheck;
    public BattleCardSelect CreateData() => new(this);
    public bool IsTrueSelect(BattleUnit source, BattleUnit target)
    {
        return selectCheck.IsTrueSelect(source, target);
    }
}
