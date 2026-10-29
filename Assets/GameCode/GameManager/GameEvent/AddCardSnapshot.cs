using System;
using System.Collections.Generic;

public class AddCardSnapshot
{
    public Type roundType;
    public List<BattleCard> addList = new();
    AddCardSnapshot() { }
    public static AddCardSnapshot Create<T>() where T : IBattleEvent
    {
        var snapshot = new AddCardSnapshot();
        snapshot.roundType = typeof(T);
        return snapshot;
    }
}
