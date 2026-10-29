using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleCardModify_BattleTimming : GameModify
{
    public bool needSettle;
    public BattleCardModified current;
    public BattleTiming timming;
    [SerializeReference] public List<GameModify> modifys = new();
    public BattleCardModify_Skill active;
    public BattleCardModify_BattleTimming Start(BattleCardModified current, BattleTiming battleTimming, bool needSettle)
    {
        this.timming = battleTimming;
        this.current = current;
        this.needSettle = needSettle;
        ModifyAndCreateResult(needSettle);

        return this;
    }
    protected override void CreateResult()
    {
        foreach (var skillAtom in timming.skills)
        {
            modifys.Add(new BattleCardModify_Skill().Start(current, skillAtom, needSettle));
        }
    }
}