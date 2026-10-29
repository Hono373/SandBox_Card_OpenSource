using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[Serializable]
public class CardInfo
{
    public static CardInfo Get(string key) => GameManager.Instance.gameDB.cardInfos[key];
    public string key;
    public string cardName;
    [SerializeReference] public BattleCardCostRule cost = new CostNo();
    [SerializeReference] public BattleCardInteractive interactive = new BattleCardInteractive_Choice();
    public ChoiceRule choiceRule = new();
    public BattleCardGraphic graphic = new();
    /// <summary>不可打出（状态牌等），卡面灰显且禁止拖拽</summary>
    public bool unplayable;
    /// <summary>虚无：回合结束时若仍留在手牌，直接移入消耗堆（PileEnum.Removed），不进弃牌堆。
    /// 跟「消耗」的区别：消耗是打出后进消耗堆（由 timing.pileEnum 表达），虚无只看结束回合时还在不在手上。</summary>
    public bool ethereal;
    public List<BattleTiming> timings = new();
    public string Desc()
    {
        // 2026-10-22 虚无是卡级关键词，按 STS 写法放描述开头（与 BattleCardModified.Desc() 保持一致）
        var desc = ethereal ? "虚无。" : "";
        foreach (var timming in timings)
        {
            desc += timming.Desc();
        }
        return desc;
    }

    [Button("验证数据")]
    public void ValidateData()
    {
        if (string.IsNullOrWhiteSpace(cardName))
            Debug.LogError($"卡牌 {key} 名称为空");
        if (cost == null)
            Debug.LogError($"卡牌 {key} cost 为空");
        else
            cost.ValidateData();
        if (interactive == null)
            Debug.LogError($"卡牌 {key} interactive 为空");
        else
            interactive.ValidateData();
        if (choiceRule == null)
            Debug.LogError($"卡牌 {key} range 为空");
        else
            choiceRule.ValidateData();
        if (graphic == null)
            Debug.LogError($"卡牌 {key} graphic 为空");
        if (timings == null || timings.Count == 0)
            Debug.LogError($"卡牌 {key} timings 为空");
        foreach (var timming in timings)
        {
            timming.ValidateData();
        }
        Debug.Log($"卡牌 {key} 数据验证完成");
    }
}
