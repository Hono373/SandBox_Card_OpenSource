using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class BattleUnitSkill : UnitIntention
{
    public override BattleUnitSkill GetResult(BattleUnit unit) => this;
    public EffectTypeEnum effectTypeEnum;
    public RangeRule range;
    public List<Skill> skills = new();
    public override UnitIntention Create()
    {
        var instance = new BattleUnitSkill();
        foreach (var skill in skills)
            instance.skills.Add(skill.Create());
        instance.range = range.Create();
        instance.effectTypeEnum = effectTypeEnum;
        CopyBaseTo(instance);
        return instance;
    }
    public override string Desc()
    {
        return string.Concat(skills.ConvertAll(skill => skill.IntentionDesc()));
    }
    /// <summary>
    /// 按当前 modifier 预演每条技能的意图数值（供 UI 显示）。
    /// 顺序与结算一致：先在技能副本上跑施法者侧 ActiveModify（力量/虚弱/脆弱），
    /// 再跑目标侧 PassiveModify（易伤等）。全部在克隆体上计算，不会污染用于结算的 skills。
    /// </summary>
    public List<string> PreviewIntentionDesc(BattleUnit caster)
    {
        var result = new List<string>();
        if (skills == null || skills.Count == 0)
        {
            GameDebug.Log($"{nameof(BattleUnitSkill)}.PreviewIntentionDesc: skills 为空，无法预演意图");
            return result;
        }

        foreach (var skill in skills)
        {
            if (skill == null || skill.effect == null)
            {
                GameDebug.Log($"{nameof(BattleUnitSkill)}.PreviewIntentionDesc: 存在无效 skill（null 或 effect 为 null）");
                result.Add(string.Empty);
                continue;
            }

            var preview = skill.Create();
            preview.effect.ActiveModify(caster, false);

            var target = PreviewTarget(preview, caster);
            if (target != null)
                preview.effect.PassiveModify(caster, target, false);

            result.Add(preview.IntentionDesc());
        }
        return result;
    }
    /// <summary>
    /// 预演目标：取范围规则解析出的第一个存活目标（敌人意图通常只有玩家一个可选目标）。
    /// 返回 null 表示当前没有有效目标，属于正常状态（如目标已全部死亡），此时不做目标侧修正。
    /// </summary>
    static BattleUnit PreviewTarget(Skill skill, BattleUnit caster)
    {
        var list = skill.rangeRule.GetTargetList(caster);
        if (list.Count == 0) return null;
        return list[0];
    }
}
