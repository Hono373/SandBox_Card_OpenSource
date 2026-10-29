using System;
using Sirenix.OdinInspector;

[Serializable]
public class ValueBonus : IBonusMode
{
    [MinValue(1)]
    public int baseBonus = 1;
}
