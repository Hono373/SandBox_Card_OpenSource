using Sirenix.OdinInspector;
using UnityEngine;

public class ModifierAdd : SkillEffect
{
    [ValueDropdown(nameof(GetAllModifierInfoKeys))]
    public string key;
    [MinValue(1)] public int layer = 1;
    public bool only;
    public override BattleFlow Start(BattleUnit caster, BattleUnit target, SkillEffectResult result)
    {
        if (result.blocked)
            return new NullFlow().Start();
        return new ModifierAddFlow().Start(this, caster, target,result);
    }
    ValueDropdownList<string> GetAllModifierInfoKeys() => InfoExtension.GetDropdownList<ModifierInfo>(info => new KeyDescPair(info.key, info.modifierName));
    public override void ValidateData()
    {
        if (string.IsNullOrWhiteSpace(key))
            Debug.LogError($"ModifierAdd key 为空");
        if (layer <= 0)
            Debug.LogError($"ModifierAdd layer 小于等于 0");
    }
    public override string Desc() => $"创建 {layer} 层 {key}";

    public override SkillEffect Create() => new ModifierAdd() { hasSource = hasSource, effectAnimData = effectAnimData, key = key, layer = layer, only = only };

    public override GameModify ActiveModify(BattleUnit caster, bool needSettle)
    {
        return new ActiveModify_ModifierAdd().Start(caster, this, needSettle);
    }

    public override GameModify PassiveModify(BattleUnit caster, BattleUnit target, bool needSettle)
    {
        return new PassiveModify_ModifierAdd().Start(caster, target, this, needSettle);
    }

    public override string IntentionDesc()
    {
        return "";
    }

    public override SkillEffectResult Apply(BattleUnit caster, BattleUnit target, GameModify passiveModify)
    {
        if (passiveModify is PassiveModify_ModifierAdd passive && passive.blocked)
            return new SkillEffectResult(0, 0, 0, string.Empty) { blocked = true };
        var modifier = Modifier.Create(this, target.data.id);
        if (modifier.data.only)
        {
            var exist = target.data.modifiers.Find(m => m.data.key == modifier.data.key);
            if (exist != null)
            {
                exist.data.layer += modifier.data.layer;
                return new SkillEffectResult(0, 0, 0, exist.data.id);
            }
        }
        GameModifyCenter.AddTo(modifier, target.data.modifiers);
        return  new SkillEffectResult(0, 0, 0, modifier.data.id);
    }
}
