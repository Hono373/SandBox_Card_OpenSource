using System.Collections.Generic;
using DG.Tweening;

public class RefreshUnitShieldFlowGroup : BattleFlow
{
    /// <summary>壁垒的 buff key：身上有它时格挡不归零。</summary>
    public const string BARRICADE_KEY = "Barricade";

    public List<BattleUnit> unitList;
    public bool needSettle;
    public RefreshUnitShieldFlowGroup Start(List<BattleUnit> unitList, bool needSettle)
    {
        this.unitList = unitList;
        this.needSettle = needSettle;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        foreach (var unit in unitList)
        {
            var ui = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(unit).shieldUI;
            // 2026-10-22 壁垒：有该 buff 时保留格挡（只刷 UI 不动数据）
            if (!unit.HasModifier(BARRICADE_KEY))
                unit.data.shield = 0;
            flow.Join(ui.Refresh());
        }
    }
}
