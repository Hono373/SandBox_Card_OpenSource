using System.Collections.Generic;

public class BattleCardPremodify_OnChoice
{
    public BattleUnit choice;
    public BattleCardModified modified;
    public List<GameModify> modifys = new();
    public RefreshBattleCardUI refreshBattleCardUI = new();
    public BattleCardPremodify_OnChoice Start(BattleCardModified current, BattleUnit choice)
    {
        this.modified = current.Create();
        this.choice = choice;
        Flow();
        return this;
    }
    protected void Flow()
    {
        foreach (var timming in modified.timings)
        {
            foreach (var skill in timming.skills)
            {
                skill.effect.PassiveModify(modified.owner, choice, false);
            }
        }
        //把预演结果（modified 为指向目标后的临时副本）交给 UI 刷新，而不是用卡牌自身的 modified
        var card = modified.Card;
        if (card == null)
        {
            GameDebug.Log($"BattleCardPremodify_OnChoice: 预演副本的 id {modified.id} 在 TotalPile 中找不到对应卡牌，错误调用！");
            return;
        }
        refreshBattleCardUI.Start(card, modified);
    }
}
