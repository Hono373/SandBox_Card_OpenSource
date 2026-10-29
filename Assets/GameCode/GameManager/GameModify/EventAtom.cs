using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EventAtom
{
    public abstract EventAtom Create();
    public virtual List<string> DescList() => new List<string>();
    public abstract void Modify(TimingEnum timmingEnum, List<EventAtom> collection, BattleCardModified current);
    public virtual Sprite GetIcon() => null;
}
