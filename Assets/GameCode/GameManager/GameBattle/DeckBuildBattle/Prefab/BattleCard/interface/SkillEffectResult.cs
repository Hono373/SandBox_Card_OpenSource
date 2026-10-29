/// <summary>
/// 技能效果结算出的数值结果（供表现层消费）。
/// 由 SkillEffect.Apply 回传，再经 Start 传递给对应的 Flow。
/// 表现层按字段是否为 0 决定是否弹字，无需额外标志。
/// </summary>
public struct SkillEffectResult
{
    public int damage;     // 伤害数值
    public int heal;       // 治疗数值
    public int shieldLost; // 护盾被吸收量（受击时）
    public string modifierID; // 结算出的 Modifier ID（供表现层播放特效用）
    public bool blocked;      // 结算被被动效果拦截（如人工制品抵挡 debuff）

    public SkillEffectResult(int damage, int heal, int shieldLost,string modifierID)
    {
        this.damage = damage;
        this.heal = heal;
        this.shieldLost = shieldLost;
        this.modifierID = modifierID;
        this.blocked = false;
    }
}
