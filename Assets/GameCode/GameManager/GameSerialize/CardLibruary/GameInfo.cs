using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家牌组配置：只保存「选了哪些卡、各几张」。
///
/// 卡牌清单不序列化于此（避免陈旧快照）：运行时真源是 GameDB.cardInfos，
/// 编辑器下由卡表 CardInfoCreator（数据来自 Assets/GameDataBase/CardInfo.json，
/// 由 Luban 从 DB.xlsx 导出）实时提供。
/// </summary>
[CreateAssetMenu]
public class GameInfo : ScriptableObject
{
    public string playerKey;

    [ListDrawerSettings(CustomAddFunction = nameof(AddCardInitial))]
    public List<CardInitial> cardDeckInitial = new();

    GameInfo() { }

    public static GameInfo Get()
    {
        var info = GameAsset.GetSingletonSo<GameInfo>();
        return info;
    }

    /// <summary>卡表清单（只读视图，不序列化，实时取自卡表）。</summary>
    [ShowInInspector]
    [PropertyOrder(-1)]
    [ListDrawerSettings(IsReadOnly = true, DraggableItems = false, NumberOfItemsPerPage = 15)]
    List<KeyDescPair> DeckCardList
    {
        get
        {
            var creator = InfoCreator<CardInfo>.Get();
            return creator != null ? creator.GetKeyDescPairs() : new List<KeyDescPair>();
        }
    }

    /// <summary>列表的「+」按钮：追加一条空条目，之后在下拉里选卡。</summary>
    void AddCardInitial()
    {
        cardDeckInitial.Add(new CardInitial());
    }
}
