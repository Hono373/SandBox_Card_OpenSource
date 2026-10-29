using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

[Serializable]
public class BattleCardModify_ChoiceRule : GameModify
{
    public BattleCardModified current;
    public ChoiceRule choiceResult;
    public BattleCardModify_ChoiceRule Start(BattleCardModified current, bool needSettle)
    {
        this.current = current;
        choiceResult = current.Info.choiceRule.Create();
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {

    }
}
