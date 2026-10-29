using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;

public class SkillApplyFlow : BattleFlow
{
    public bool needSettle;
    public Skill skill;
    public BattleUnit caster;
    public BattleUnit choice;
    public bool hasSource;
    public int countResult;
    public int costUsed;
    public List<BattleUnit> targetPool;
    public List<BattleFlow> gameSteps = new();
    //常见效果：吸血

    //假设为
    //对 目标造成伤害
    //对 目标造成伤害n次
    //对 目标造成伤害x次
    //对 随机目标造成n次伤害
    //对 随机目标造成X次伤害
    //对 所有敌人造成n伤害 x次
    //对 所有敌人造成n伤害 n次
    public SkillApplyFlow Start(Skill skill, BattleUnit caster, BattleUnit choice, int costUsed, bool needSettle)
    {
        this.needSettle = needSettle;
        this.costUsed = costUsed;
        this.skill = skill;
        this.caster = caster;
        this.choice = choice;
        this.hasSource = skill.effect.hasSource;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        if (choice == null)
        {
            targetPool = skill.rangeRule.GetTargetPool(caster);
        }
        else
        {
            targetPool = new() { choice };
        }

        skill.countRule.Modify(this, true, out countResult);

        // 2026-10-21 受击被动的触发时机：既不能在 Apply 后同步触发（buff 会早于伤害动画），
        // 也不能插进 countFlow（AppendCallback 会把多段技能的并行结构改成串行，旋风斩最明显）。
        // 所以构建期只记录，整段技能动画播完后再统一结算。
        var takenDmgRecords = new List<(BattleUnit caster, BattleUnit target, int damage)>();

        for (int i = 0; i < countResult; i++)
        {
            var countFlow = GameAnim.Flow();
            var targetList = skill.rangeRule.GetTargetList(caster, targetPool);
            if (targetList.Count == 0) continue;
            countFlow.Join(EffectAnimObj.CreateAnim(skill.effect.effectAnimData).AOEAnim(skill.rangeRule.baseRange));
            foreach (var target in targetList)
            {
                if (target.current.isDead) continue;
                var effect = skill.effect.Create();
                var passiveModify = effect.PassiveModify(caster, target, needSettle);
                var result = effect.Apply(caster, target, passiveModify);
                // Apply 已消费完 context（blocked/数值），此时才结算 modifier 自身状态（扣层、移除）
                passiveModify.Settle();
                var effectFlow = effect.Start(caster, target, result);
                countFlow.Join(effectFlow.flow);
                gameSteps.Add(effectFlow);
                // 护盾全吸收时 result.damage == 0，不算受到攻击伤害，不记录。
                if (result.damage > 0)
                    takenDmgRecords.Add((caster, target, result.damage));
            }
            flow.Append(countFlow);
        }

        // 2026-10-22 受击被动：原来写在 flow.onComplete 回调里，绕过 flow 链（isGameOver / 序列中断时行为不一致）。
        // 现在包成 AfterTakeDmgFlow 接进序列末尾 —— 与「独立事件都要封装成 BattleFlow」的原则一致。
        flow.Append(new AfterTakeDmgFlow().Start(takenDmgRecords, needSettle).flow);
    }
}