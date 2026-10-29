using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 暴走：造成 value + (已有层数 × layer) 点伤害，然后层数 +1（本场战斗持续累积）。
///
/// 为什么不用卡牌自身存次数：`SkillEffect` 拿不到「是哪张卡打出的」。
/// 既然只有暴走这一个 effect 需要，就把计数挂成 caster 身上的隐藏 buff（RampageStack），
/// 用现成的 ModifierAdd 机制累加 —— 不碰 Skill / BattleCardUseFlow。
/// 代价：卡组里同时有多张暴走时共享层数（用户确认可接受）。
/// </summary>
public class RampageEffect : SkillEffect
{
    /// <summary>基础伤害（W 列）。</summary>
    [MinValue(1)] public int value = 8;
    /// <summary>每层递增（Y 列）。</summary>
    [MinValue(1)] public int layer = 5;

    public const string STACK_KEY = "RampageStack";

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"暴走 {GetType().Name} value 小于等于 0");
        if (layer <= 0)
            Debug.LogError($"暴走 {GetType().Name} layer 小于等于 0");
    }
    public override string Desc() => $"造成{value}点伤害，每打出一次伤害+{layer}";
    public override string IntentionDesc() => $"{value}";
    public override SkillEffect Create() => new RampageEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value, layer = layer };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        var stacks = 0;
        foreach (var modifier in caster.data.modifiers)
        {
            if (modifier.data.key == STACK_KEY) stacks = modifier.data.layer;
        }

        // 先按当前层数结算，再累加（本次打出不吃自己这一层）
        var total = value + stacks * layer;
        var add = new ModifierAdd { key = STACK_KEY, layer = 1, only = true };
        add.Apply(caster, caster, null);

        var overflow = target.data.shield - total;
        if (overflow >= 0)
        {
            target.data.shield = overflow;
            return new SkillEffectResult(damage: 0, heal: 0, shieldLost: total, string.Empty);
        }

        var shieldAbsorb = target.data.shield;
        // 2026-10-22 飘字/吸血用「格挡后的真实伤害」realDamage，不再以目标剩余血量为上限
        var realDamage = -overflow;
        var hpAfter = Mathf.Max(0, target.data.hp - realDamage);
        target.data.shield = 0;
        target.data.hp = hpAfter;
        return new SkillEffectResult(damage: realDamage, heal: 0, shieldLost: shieldAbsorb, string.Empty);
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new DamageFlow().Start(new Damage { effectAnimData = effectAnimData }, caster, target, result);
}
