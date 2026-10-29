using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 受伤后事件流程：把一次技能结算里所有「实际造成伤害」的目标汇总后广播 AfterTakeDmg。
///
/// 2026-10-22 从 SkillApplyFlow.flow.onComplete 回调里搬出来 —— 回调里的代码绕过 flow 链，
/// 一旦 isGameOver 或序列被中断，它的行为与 flow 不一致（用户原则：独立事件都要包成 BattleFlow）。
/// </summary>
public class AfterTakeDmgFlow : BattleFlow
{
    /// <summary>(caster, target, 实际扣血量)。护盾全吸收时不入列。</summary>
    public List<(BattleUnit caster, BattleUnit target, int damage)> records = new();
    public bool needSettle;

    public AfterTakeDmgFlow Start(List<(BattleUnit caster, BattleUnit target, int damage)> records, bool needSettle)
    {
        this.records.AddRange(records);
        this.needSettle = needSettle;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        // 事件必须在「动画序列走到这一步」时才广播（受击被动要在伤害演出之后结算），
        // 所以包成 AppendCallback，而不是构建期直接调用。
        flow.AppendCallback(() =>
        {
            foreach (var record in records)
            {
                new AfterTakeDmg().Start(record.caster, record.target, record.damage, needSettle);
            }
        });
    }
}
