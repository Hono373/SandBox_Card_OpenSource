using System;
using TMPro;
using UnityEngine.UI;
public class BattleCardCostUIData
{
    public bool active;
    public string value;
    public BattleCardCostUIData(bool active, string value)
    {
        this.active = active;
        this.value = value;
    }
}