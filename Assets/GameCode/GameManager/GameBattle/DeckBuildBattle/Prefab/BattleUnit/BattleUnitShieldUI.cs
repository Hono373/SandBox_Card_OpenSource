using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnitShieldUI : MonoBehaviour
{
    bool isShow;
    BattleUnitUI unitObj;
    /// <summary>
    /// 2026-10-23 护盾图标的原始锚点。upAnim 是 JumpAnim，目标是 anchoredPosition.x + offsetX（相对），
    /// 不复位的话每次 Show 都会在上一次落点上再 +offsetX，护盾一掉一涨就一路往右漂。
    /// </summary>
    Vector2 homePos;
    [HideInInspector] public Image img;
    [HideInInspector] public TextMeshProUGUI value;
    UIGroup uiGroup;
    [SerializeReference] UIAnim upAnim;
    [SerializeReference] UIAnim enterAnim;
    [SerializeReference] UIAnim leaveAnim;
    public void Init(BattleUnitUI unitObj)
    {
        this.unitObj = unitObj;
        this.GetComponentSafety(out img);
        UIGroup.Create(this, out uiGroup);
        this.CheckInChild(out value);
        homePos = uiGroup.RectT.anchoredPosition;
        uiGroup.SetActive(false);
    }
    public Sequence Refresh()
    {
        value.text = unitObj.manager.data.shield.ToString();

        if (unitObj.manager.data.shield <= 0)
        {
            return Leave();
        }

        if (isShow)
        {
            return GameAnim.Flow();
        }
        else
        {
            return Show();
        }
    }
    public Sequence Show()
    {
        isShow = true;
        img.sprite = GameAsset.GetSprite("Shield");
        // 先复位再播：upAnim 是相对位移，不复位会累积
        uiGroup.RectT.anchoredPosition = homePos;
        var sequeue = GameAnim.Flow(nameof(Show));
        sequeue.Join(enterAnim.Tween(uiGroup));
        sequeue.Join(upAnim.Tween(uiGroup));
        return sequeue;
    }
    public Sequence Leave()
    {
        isShow = false;
        var sequeue = GameAnim.Flow(nameof(Leave));
        img.sprite = GameAsset.GetSprite("ShieldBreak");
        sequeue.Join(leaveAnim.Tween(uiGroup));
        return sequeue;
    }
}
