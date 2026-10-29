using DG.Tweening;

public class ModifierAddFlow : BattleFlow
{
    public ModifierAdd modifierAdd;
    public BattleUnit caster;
    public BattleUnit target;
    public SkillEffectResult skillEffectResult;
    public ModifierAddFlow Start(ModifierAdd modifierAdd, BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        this.modifierAdd = modifierAdd;
        this.caster = caster;
        this.target = target;
        this.skillEffectResult = result;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        //modifier 变化会影响意图数值（施法者力量、目标易伤等），这里同步重算敌方意图显示（不重抽意图）
        RefreshEnemyIntentions();

        var modifier = GameModifyCenter.modifiers[skillEffectResult.modifierID];
        if (modifier.GetInfo().hidden)
            return;
        var ui = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(target).modifierBarUI;
        flow.onComplete += () => ui.AddOrRefresh(modifier);
    }
    /// <summary>按当前 modifier 重算所有存活敌人的意图数值显示，不重新抽取意图。</summary>
    void RefreshEnemyIntentions()
    {
        foreach (var enemy in DeckBuildBattle.Active.data.GetEnemyList())
        {
            if (enemy.current.isDead) continue;
            flow.Join(new RefreshUnitIntentionFlow().Start(enemy).flow);
        }
    }
}
