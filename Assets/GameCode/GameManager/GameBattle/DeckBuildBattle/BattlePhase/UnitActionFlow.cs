using System.Collections.Generic;
using DG.Tweening;
public class UnitActionFlow : BattleFlow
{
    public BattleUnit unit;
    public BattleEndCheckFlow deathCheckModify = new();
    public RefreshBattleCardGroupFlow refreshBattleCardGroupFlow = new();
    public BattleUnitActionMove actionMove = new();
    public UnitActionFlow Start(BattleUnit unit)
    {
        this.unit = unit;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        if (DeckBuildBattle.Active.isGameOver) return;
        if (unit.current.isDead) return;
        // 2026-10-20 历史意图必须在"播放时点"写，不能在这个 FlowBuild 里同步 Add：
        // UnitActionPhaseGroupFlow 构建期就把整组单位的 flow 一次建完，排在后面的单位
        // 可能在前面的单位行动中已经被打死，构建期写会记成"这一招用过了"。
        flow.AppendCallback(unit.RecordSkillHistory);
        // 2026-10-20 仅攻击型行动前冲，技能/能力不位移；整个行动只冲一次，放在技能结算之前。
        if (unit.unitSkill.effectTypeEnum == EffectTypeEnum.Dmg)
            flow.Append(actionMove.Start(unit).flow);
        foreach (var skill in unit.unitSkill.skills)
        {
            //与卡牌结算保持一致：先在技能副本上跑施法者侧 ActiveModify（力量/虚弱），
            //目标侧 PassiveModify（易伤等）由 SkillApplyFlow 内部处理。
            //缺了这一步敌人力量/虚弱完全不生效，也会与意图显示（PreviewIntentionDesc）不一致。
            var activeSkill = skill.Create();
            var activeModify = activeSkill.effect.ActiveModify(unit, needSettle: true);
            activeModify.Settle();

            var skillPhase = new SkillApplyFlow().Start(activeSkill, unit, activeSkill.rangeRule.AIChoice(unit), 0, needSettle: true);
            flow.Join(skillPhase.flow);
        }
        flow.Append(refreshBattleCardGroupFlow.Start(needSettle: false).flow);
        // 2026-10-21 行动播完刷新自己的意图。BattlePhaseStart 一场战斗只进一次，回合循环从 EnemyPhaseEnd
        // 直接回 PlayerPhaseStart，所以只挂在 BattlePhaseStart 上会让 unitSkill 永远停在开局那次计算，
        // skillHistory 记了也没人再读，条件筛选取到的历史恒为空。
        flow.AppendCallback(unit.RefreshIntention);
        flow.Append(deathCheckModify.Start().flow);
    }
}
