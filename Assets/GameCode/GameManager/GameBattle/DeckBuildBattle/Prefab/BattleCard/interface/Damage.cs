using Sirenix.OdinInspector;
using UnityEngine;

public class Damage : SkillEffect
{
    [MinValue(0)] public int value;
    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"伤害 {GetType().Name} value 小于等于 0");
    }
    public override string Desc() => $"造成{value}点伤害";
    public override string IntentionDesc() => $"{value}";
    // 2026-10-23 hasSource 必须一起复制：表里配的 hasSource=false（无来源 = 自伤）原来在 Create 时被丢掉，
    // 运行时恒为 true，于是力量/虚弱/易伤把自伤也当成「造成的伤害」一起修正（祭品掉血变成 6+力量）。
    public override SkillEffect Create() => new Damage() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle)
    {
        return new ActiveModify_Damage().Start(caster, this, needSettle);
    }
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle)
    {
        return new PassiveModify_Damage().Start(caster, target, this, needSettle);
    }
    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        // 2026-10-22 自伤 = STS 的「失去 N 点生命」，无视护盾。
        // 判据用 caster == target，不加 ignore 字段 —— reason：effect 子字段被表头合并区
        // U4:AC4 锁在 U~AC 九列且已排满，加字段要插列 + 重设三个合并区，代价不划算。
        // 当前所有自伤卡（放血/残暴/自燃/御血术/祭品）都是 baseRange=Self、无来源，判断成立；
        // 判据用 caster == target 而不是 hasSource：万一某张 Self 卡漏配 hasSource=false，
        // 用配置判会把自伤当成普通伤害（被自己护盾吸收），用运行时判据更抗漏配。
        // 若将来出现 baseRange=All 的伤害牌（会把自己算进目标池），这里需要再收窄。
        if (caster == target)
        {
            // 2026-10-22 显示的失血量用 value 本身，不按「剩余血量」截断（血不够也照实掉这么多）
            var hpAfterLose = Mathf.Max(0, target.data.hp - value);
            target.data.hp = hpAfterLose;
            // 2026-10-22 失去生命事件（撕裂订阅）—— 只有走"无视护盾"的自伤路径才发
            new AfterLoseHp().Start(target, value, true).PlayFlows();
            return new SkillEffectResult(damage: value, heal: 0, shieldLost: 0, string.Empty);
        }

        var overflow = target.data.shield - value;
        if (overflow >= 0)
        {
            target.data.shield = overflow;
            return new SkillEffectResult(damage: 0, heal: 0, shieldLost: value,string.Empty);
        }

        var shieldAbsorb = target.data.shield;
        // 2026-10-22 飘字/吸血用「格挡后的真实伤害」realDamage，不再以目标剩余血量为上限
        var realDamage = -overflow;
        var hpAfter = Mathf.Max(0, target.data.hp - realDamage);
        target.data.shield = 0;
        target.data.hp = hpAfter;
        return new SkillEffectResult(damage: realDamage, heal: 0, shieldLost: shieldAbsorb,string.Empty);
    }
    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        return new DamageFlow().Start(this, caster, target, result);
    }
}