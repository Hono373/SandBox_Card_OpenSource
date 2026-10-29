using Sirenix.OdinInspector;
using UnityEngine;

public class AddCostEffect : SkillEffect
{
    [MinValue(0)] public int value;
    // 2026-10-22 加费没有前置 Modify 需求，但调用方（ActiveModify_*/SkillApplyFlow）会直接 .Settle()，
    // 返回 null 会让开局 RefreshBattleCardGroupFlow 遍历卡组时 NRE，这里统一给空 modify。
    public override GameModify ActiveModify(BattleUnit caster, bool needSettle)
    {
        return new NullGameModify();
    }

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        DeckBuildBattle.Active.data.energy += value;
        return default;
    }

    public override SkillEffect Create()
    {
        return new AddCostEffect() { hasSource = hasSource, effectAnimData = effectAnimData, value = value };
    }

    public override string Desc()
    {
        return $"获得{value}点费用";
    }

    public override string IntentionDesc()
    {
        return "";
    }

    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle)
    {
        return new NullGameModify();
    }

    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        return new AddCostFlow().Start(this, caster, target);
    }

    public override void ValidateData()
    {
        if (value <= 0)
            Debug.LogError($"加费 {GetType().Name} value 小于等于 0");
    }
}
