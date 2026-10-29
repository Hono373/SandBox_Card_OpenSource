using System;

/// <summary>
/// 费用接口
/// </summary>
[Serializable]
public abstract class BattleCardCostRule
{
    public abstract BattleCardCostRule Create();
    public abstract int Cost();
    public abstract int Apply(int modify);
    public abstract BattleCardCostUIData RefreshCostUI();
    public abstract int GetResult();
    public virtual void ValidateData() { }
}