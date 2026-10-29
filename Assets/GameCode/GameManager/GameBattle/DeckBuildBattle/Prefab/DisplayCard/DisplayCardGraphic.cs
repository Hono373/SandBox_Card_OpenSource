using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class DisplayCardGraphic : MonoBehaviour
{
    [HideInInspector] public UIGroup uiGroup;
    public Image bg;
    public Image banner;
    public Image img;
    public Image frame;
    public BattleCardCostUI costUI;
    public TextMeshProUGUI Cost;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Describe;

    public void Init()
    {
        UIGroup.Create(this, out uiGroup);
    }
    public void SetActive(bool v) => uiGroup.SetActive(v);

    internal void Fresh(CardInfo info)
    {
        Name.text = info.cardName;
        if (costUI != null)
            costUI.Refresh(info.cost.RefreshCostUI());   // 原始费用
        else if (Cost != null)
            Cost.text = info.cost.RefreshCostUI().value;

        if (info.graphic != null)
        {
            var graphic = info.graphic.Create();
            if (bg != null) bg.sprite = graphic.CardBgResult();
            if (banner != null) banner.sprite = graphic.BannerResult();
            if (img != null) img.sprite = GameAsset.GetSprite($"Card_{info.key}");
            if (frame != null) frame.sprite = graphic.FrameResult();
        }

        if (Describe != null)
            Describe.text = info.Desc();   // 原始描述（直接遍历 info 的原始 EffectAtom，不经 Modify）
    }
}
