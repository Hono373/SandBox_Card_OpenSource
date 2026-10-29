using UnityEngine;

/// <summary>
/// 卡牌对象外观刷新的一次性数据快照（仅使用一次）。
/// 在发起刷新时立即从传入的 BattleCardModified 采集外观数据，冻结调用时刻的状态；
/// 动画队列延迟执行时不再读取 BattleCard.modified，避免触发时它已被多次更新导致外观错乱。
/// 数据源由调用方显式指定：常规刷新传 card.modified；指向目标等预演场景传临时副本，
/// 否则预演结果（如易伤加成后的伤害）不会显示在卡面上。
/// </summary>
public class BattleCardUIShot
{
    public string cardName;
    public BattleCardCostUIData costUIData;
    public string desc;
    public string type;
    public Sprite bg;
    public Sprite rare;
    public Sprite img;
    public Sprite frame;
    public bool useBlock;
    /// <summary>modified：要渲染的外观数据源，可为预演用的临时副本。</summary>
    public BattleCardUIShot(BattleCardModified modified)
    {
        if (modified == null)
        {
            GameDebug.Log($"{nameof(BattleCardUIShot)}: 传入的 BattleCardModified 为 null，卡面不会刷新！");
            return;
        }
        var graphic = modified.graphic;

        cardName = modified.cardName;
        costUIData = modified.costRule.RefreshCostUI();
        desc = modified.Desc();
        type = graphic.TypeResult();
        bg = graphic.CardBgResult();
        rare = graphic.BannerResult();
        img = GameAsset.GetSprite($"Card_{modified.key}");
        frame = graphic.FrameResult();
        useBlock = modified.useBlock;

    }
}
