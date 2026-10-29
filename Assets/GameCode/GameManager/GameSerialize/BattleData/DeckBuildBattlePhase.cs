using DG.Tweening;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class DeckBuildBattlePhase
{
    [SerializeReference] public BattlePhase current;
    public static DeckBuildBattlePhase Create()
    {
        var instance = new DeckBuildBattlePhase();
        instance.current = new BattlePhaseStart();
        return instance;
    }
    public void Start()
    {

        current.Start();
    }
}
