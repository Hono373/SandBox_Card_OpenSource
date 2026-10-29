using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class BattleCardCostUI
{
    public TextMeshProUGUI tmp;
    public Image bg;
    public void SetActive(bool v)
    {
        bg.gameObject.SetActive(v);
    }
    public void Refresh(BattleCardCostUIData data)
    {
        SetActive(data.active);
        tmp.text = data.value.ToString();
    }
}