using System;
using System.Collections.Generic;
using DG.Tweening;

[Serializable]
public class BattleCardModify_Key : GameModify
{
    public BattleCardModify phase;
    public SortedWeight<string> keyWeight = new();
    public string keyResult;
    public BattleCardModify_Key Start(BattleCardModify phase, bool needSettle)
    {
        this.phase = phase;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {
        keyResult = keyWeight.MaxOrDefault(phase.modified.Card.data.key);
    }
}
