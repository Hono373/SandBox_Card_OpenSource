using System;
using System.Collections.Generic;
using Newtonsoft.Json;
[Serializable]
public class GameData
{
    public UnitLibrary unitLibrary;
    public CardLibrary cardLibrary;
    public PlayerData playerData;
    public DeckBuildBattle deckBuildBattle;
    public Queue<string> storyQueue = new();
    public Dictionary<string, Modifier> modiferDict = new();
    public SceneSerialize sceneData = null;
    GameData() { }
    public static GameData Create(GameInfo gameInfo)
    {
        var instance = new GameData();
        instance.unitLibrary = UnitLibrary.Create(gameInfo);
        instance.cardLibrary = CardLibrary.Create(gameInfo);
        instance.playerData = PlayerData.Create(gameInfo);
        return instance;
    }
    public void CreateDDB(DeckBuildBattleInfo info)
    {
        deckBuildBattle = DeckBuildBattle.Create(info);
    }
}
