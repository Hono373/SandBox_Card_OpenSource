using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattleCard : IInfiniteData
{
    public BattleCard() { }
    internal static BattleCard Create(string key, string id, string playerKey, bool temprorary)
    {
        var instance = new BattleCard();
        instance.data = BattleCardData.Create(key, id, playerKey, temprorary);
        return instance;
    }

    [SerializeField][JsonProperty] public BattleCardData data;
    [JsonIgnore] public MotionData argData;
    [JsonIgnore] public BattleCardModified modified;
}
