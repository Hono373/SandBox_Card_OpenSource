using System;
using DG.Tweening;
using Newtonsoft.Json;

[Serializable]
public class BattleCardModify_CostRule : GameModify
{
    /// <summary>腐化的 buff key：技能牌耗能归零、打出即消耗。</summary>
    public const string CORRUPTION_KEY = "Corruption";

    public BattleCardModified modified;
    public BattleCardCostRule cost;
    public int costModify;
    public int costResult;
    public BattleCardModify_CostRule() { }
    public BattleCardModify_CostRule Start(BattleCardModified current, bool needSettle)
    {
        this.modified = current;
        cost = current.costRule;
        ModifyAndCreateResult(needSettle);
        return this;
    }
    protected override void CreateResult()
    {
        // 2026-10-22 腐化：技能牌耗能归零（先于 cost.Apply，避免被费用修正再次改写）。
        // ⚠️ 本方法执行时机在 BattleCardModify.Start 的中途（costRuleModify 那一步），
        // 那时只有排在它之前的字段可用：id / key / cardName / owner / frequency / costRule，
        // 以及靠 key 派生的 Info。**graphic / timings / interaction 都还没赋值**（都是 null）。
        // 两次 NRE 的教训：先读 modified.owner（null），再读 modified.graphic（null）。
        if (modified.Info.graphic.type == TypeEnum.Skill && MateHasCorruption())
        {
            costResult = 0;
            return;
        }
        costResult = cost.Apply(costModify);
    }

    /// <summary>玩家（Mate 阵营）身上是否有腐化。</summary>
    public static bool MateHasCorruption()
    {
        foreach (var unit in DeckBuildBattle.Active.data.unitDict.GetByCamp(CampEnum.Mate))
        {
            if (unit.HasModifier(CORRUPTION_KEY)) return true;
        }
        return false;
    }
}
