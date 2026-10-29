using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 巩固：把当前格挡乘以 value（默认 2 = 翻倍）。
/// value 走 W 列。作用于自己（Hold 卡 baseRange=Self，target 就是 caster）。
/// </summary>
public class ShieldTimesEffect : SkillEffect
{
    /// <summary>倍数（W 列）。</summary>
    [MinValue(2)] public int value = 2;

    public override void ValidateData()
    {
        if (value < 2)
            Debug.LogError($"格挡倍率 {GetType().Name} value 小于 2，没有意义");
    }
    public override string Desc() => $"将格挡乘以{value}";
    public override string IntentionDesc() => "";
    public override SkillEffect Create() => new ShieldTimesEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle) => new NullGameModify();
    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle) => new NullGameModify();

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        target.data.shield = target.data.shield * value;
        return default;
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
        => new ShieldFlow().Start(new Shield { effectAnimData = effectAnimData }, caster, target);
}
