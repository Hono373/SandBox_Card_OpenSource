using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BattleCardModified
{
    public BattleCard Card => DeckBuildBattle.Active.deck.dict[PileEnum.TotalPile].Get(id);
    // 2026-10-22 移除 keyModify / frequencyModify：它们是 BattleCardModify 自己的中间产物，结果落到
    // key / frequency 字段即用完；留在这里会和 BattleCardModify_Key.phase、
    // BattleCardModify_Frequency.cardModified 互指形成环，触发 Unity「序列化深度 10 超限」。
    public string id;
    public string key;
    public string cardName;
    public BattleUnit owner;
    public CardInfo Info => CardInfo.Get(key);
    public int costResult;
    [SerializeReference] public BattleCardCostRule costRule;
    [SerializeReference] public BattleCardInteractive interactiveMode;
    public ChoiceRule rangeRule;
    public BattleCardGraphic graphic;
    public List<BattleTiming> timings = new();
    public int frequency;
    public bool useBlock;
    public string useBlockDesc;

    public BattleCardModified() { }
    public BattleCardModified Create()
    {
        var instance = new BattleCardModified();

        instance.id = id;
        instance.key = key;
        instance.cardName = cardName;
        instance.frequency = frequency;

        instance.costResult = costResult;
        instance.costRule = costRule.Create();
        instance.interactiveMode = interactiveMode.Create();
        instance.rangeRule = rangeRule.Create();
        instance.graphic = graphic.Create();

        foreach (var timing in timings)
        {
            instance.timings.Add(timing.Create());
        }

        instance.owner = owner;
        instance.useBlock = useBlock;
        instance.useBlockDesc = useBlockDesc;

        return instance;
    }
    public string Desc()
    {
        // 2026-10-22 虚无是卡级关键词，按 STS 写法放描述开头（与 CardInfo.Desc() 保持一致）
        var desc = Info.ethereal ? "虚无。" : "";
        foreach (var timing in timings)
        {
            desc += timing.Desc();
        }
        return desc;
    }
}
