using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCardUI : MonoBehaviour
{
    [SerializeReference] public BattleCard manager;
    public BattleCardHandler handler;
    public AnimController animOverride;
    [SerializeReference] public UIAnim createAnim;
    [SerializeReference] public UIAnim leaveAnim;
    public CardStateEnum stateEnum;
    public Image useableEffect;
    public void InUseRange()
    {
        useableEffect.color = new(0.75f, 0.75f, 0, 1);
    }
    public void Useable()
    {
        if (manager.modified.useBlock)
        {
            useableEffect.color = new(1, 1, 1, 0);
        }
        else
        {
            useableEffect.color = new(0, 0.8f, 1, 1);
        }
    }
    public Image bg;
    public Image rare;
    public Image img;
    public Image frame;
    public TextMeshProUGUI type;
    public TextMeshProUGUI cardCost;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDesc;
    public BattleCardCostUI costUI;
    public CanvasGroup CanvasGroup() => cg ??= this.GetCanvasGroup();
    CanvasGroup cg;
    public RectTransform RectTransform() => rectT ??= RectTransformExtensions.GetRectTransform(this);
    RectTransform rectT;
    public void Init(BattleCard card)
    {
        this.manager = card;

        CanvasGroup();
        RectTransform();

        this.GetComponentSafety(out handler).Init(this);

        animOverride = new AnimController();

        cg.SetActive(false);
        Refresh(new(manager.modified));
    }
    public void Refresh(BattleCardUIShot snapshot)
    {
        cardName.text = snapshot.cardName;
        costUI.Refresh(snapshot.costUIData);
        cardDesc.text = snapshot.desc;

        type.text = snapshot.type;
        bg.sprite = snapshot.bg;
        rare.sprite = snapshot.rare;
        img.sprite = snapshot.img;
        frame.sprite = snapshot.frame;
        Useable();
    }
    public Vector2 GetMousePos()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle
             (
             transform.parent as RectTransform,
             Input.mousePosition,
             GameCamera.GetObj().camera,
             out Vector2 mousePosition
             );
        return mousePosition;
    }
    public void SetOriginalSiblingIndex()
    {
        var n = DeckBuildBattle.Active.deck.GetPile(PileEnum.Hand).list.IndexOf(manager);
        transform.SetSiblingIndex(n);
    }
    public void Destroy()
    {
        animOverride.Destroy();
        this.stateEnum = CardStateEnum.Destroy;
        cg.Ray(false);
    }
    public static BattleCardObjAnim Anim => BattleCardObjAnim.Get();
    public static BattleCardObjInfo Info() => BattleCardObjInfo.Get();
}
