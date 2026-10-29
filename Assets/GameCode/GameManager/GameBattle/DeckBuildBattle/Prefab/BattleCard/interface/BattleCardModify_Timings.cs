using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleCardModify_Timings : GameModify
{
    public bool needSettle;
    public BattleCardModified modified;
    public List<BattleCardModify_BattleTimming> modifys = new();
    public BattleCardModify_Timings Start(BattleCardModified modified, bool needSettle)
    {
        this.needSettle = needSettle;
        this.modified = modified;
        foreach (var timing in this.modified.Info.timings)
        {
            this.modified.timings.Add(timing.Create());
        }
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {
        foreach (var timming in modified.timings)
        {
            modifys.Add(new BattleCardModify_BattleTimming().Start(modified, timming, needSettle));
        }
    }
}
