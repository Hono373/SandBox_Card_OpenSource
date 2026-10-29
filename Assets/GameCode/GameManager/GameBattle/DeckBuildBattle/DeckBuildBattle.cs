using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class DeckBuildBattle
{
    public static DeckBuildBattle Active => GameSerialize.Active.deckBuildBattle;
    public bool isGameOver;
    public DeckBuildBattleData data;
    public DeckBuildBattleDeck deck;
    public DeckBuildBattlePhase phase;
    /// <summary>
    /// 常驻在场的能力牌（graphic.type == Ability）。打出时由 BattleTiming.AfterUse 登记，
    /// 不进弃牌堆、不被洗回抽牌堆；相位切换时由 CardPhaseTimingFlow 触发其 timings。
    /// </summary>
    public List<BattleCard> powers = new();
    [JsonIgnore] public DeckBuildBattleUI obj;
    DeckBuildBattle() { }

    public void AddPower(BattleCard card)
    {
        if (card == null) return;
        if (powers.Contains(card)) return;
        powers.Add(card);
    }

    public void RemovePower(BattleCard card)
    {
        if (card == null) return;
        powers.Remove(card);
    }

    public void ClearPowers() => powers.Clear();
    public static DeckBuildBattle Create(DeckBuildBattleInfo info)
    {
        var instance = new DeckBuildBattle();
        instance.data = DeckBuildBattleData.Create(GameSerialize.Active, info);
        instance.deck = DeckBuildBattleDeck.Create(GameSerialize.Active.playerData.cardPile, GameSerialize.Active.playerData.cardDict);
        instance.phase = DeckBuildBattlePhase.Create();
        return instance;
    }
    public static DeckBuildBattleUI SetObj()
    {
        if (Active.obj != null)
        {
            GameDebug.Log("DeckBuildBattleObj 已存在，错误调用！");
        }
        Active.obj = GameAsset.Instantiate<DeckBuildBattleUI>(GameManager.Instance.container);
        Active.obj.Init(Active);
        return Active.obj;
    }
    public static DeckBuildBattleUI GetUI()
    {
        if (Active.obj == null)
        {
            GameDebug.Log("DeckBuildBattleObj 不存在，错误调用！");
            SetObj();
        }
        return Active.obj;
    }
    public static DeckBuildBattleRule GetRule() => DeckBuildBattleRule.Get();
    public DeckBuildBattleInfo GetInfo() => DeckBuildBattleInfo.Get(data.key);
}
