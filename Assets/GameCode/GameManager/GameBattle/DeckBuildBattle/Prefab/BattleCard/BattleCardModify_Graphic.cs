using System;
using DG.Tweening;

[Serializable]
public class BattleCardModify_Graphic : GameModify
{
    public BattleCardModified current;
    public BattleCardGraphic graphicResult;
    public BattleCardModify_Graphic Start(BattleCardModified current, bool needSettle)
    {
        this.current = current;
        graphicResult = current.Info.graphic.Create();
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected sealed override void CreateResult()
    {


    }
}
