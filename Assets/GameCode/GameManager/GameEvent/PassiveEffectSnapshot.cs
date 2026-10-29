public class PassiveEffectSnapshot
{
    public BattleUnit target;
    public BattleUnit source;

    PassiveEffectSnapshot() { }
    public PassiveEffectSnapshot(BattleUnit target, BattleUnit source) { this.target = target; this.source = source; }
}
