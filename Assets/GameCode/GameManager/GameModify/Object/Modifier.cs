using Newtonsoft.Json;
using System;
using System.Collections.Generic;
[Serializable]
public class Modifier
{
    [JsonProperty] public ModifierData data;
    [JsonProperty] public List<ModifierEffect> modifierEffects = new();
    public static Modifier Create(ModifierAdd request, string owner)
    {
        var info = ModifierInfo.Get(request.key);
        var instance = new Modifier();
        instance.data = new ModifierData()
        {
            id = GameModifyCenter.CreateID(),
            key = request.key,
            owner = owner,
            layer = request.layer,
            only = request.only || info.only,
            paramsDict = new()
        };
        foreach (var effect in info.modifierEffects)
        {
            instance.modifierEffects.Add(effect.Create());
        }
        foreach (var effect in instance.modifierEffects)
        {
            effect.Init(instance);
        }
        return instance;
    }
    public BattleUnit GetOwner() => DeckBuildBattle.Active.data.unitDict.GetBattleUnitById(data.owner);
    public ModifierInfo GetInfo() => ModifierInfo.Get(data.key);
}
