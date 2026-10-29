using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BattleUnitIntentionUI : MonoBehaviour, IGameUI
{
    Image flag;
    TextMeshProUGUI flagValue;
    CanvasGroup cg;
    public CanvasGroup CG => cg ??= this.GetCanvasGroup();
    public void Init()
    {
        this.CheckInChild(out flag);
        this.CheckInChild(out flagValue);
    }

    public void SetActive(bool v)
    {
        CG.SetActive(v);
    }
    public void Clear()
    {
        SetActive(false);
        flag.sprite = null;
        flagValue.text = "";
    }
    /// <summary>
    /// 刷新意图显示。数值走 BattleUnitSkill.PreviewIntentionDesc 预演，
    /// 这样力量/虚弱（施法者侧）与易伤（目标侧）等 modifier 会体现在意图数字上。
    /// </summary>
    public void Refresh(BattleUnit unit)
    {
        if (unit == null)
        {
            GameDebug.Log("BattleUnitIntentionUI.Refresh: unit 为 null，错误调用！");
            Clear();
            return;
        }
        var unitSkill = unit.unitSkill;
        if (unitSkill == null)
        {
            GameDebug.Log($"BattleUnitIntentionUI.Refresh: 单位 {unit.data.key} 的 unitSkill 为 null，错误调用！");
            Clear();
            return;
        }
        flag.sprite = BattleIconInfo.GetIcon(unitSkill.effectTypeEnum);
        flagValue.text = string.Concat(unitSkill.PreviewIntentionDesc(unit));
        SetActive(true);
    }
}