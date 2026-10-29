public class PassiveHealModifier : PassiveValueModifier
{
    private PassiveHealModifier() { }
    public PassiveHealModifier(BattleUnit target) { this.target = target; }
}

