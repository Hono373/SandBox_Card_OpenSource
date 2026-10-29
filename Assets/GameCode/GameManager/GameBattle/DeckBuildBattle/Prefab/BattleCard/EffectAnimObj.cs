using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class EffectAnimObj : MonoBehaviour
{
    public Image img;
    [SerializeField] EffectAnimData data;
    public RectTransform RectT() => rectT ??= this.GetRectTransform();
    RectTransform rectT;
    public CanvasGroup Cg() => cg ??= this.GetCanvasGroup();
    CanvasGroup cg;
    public static EffectAnimObj CreateAnim(EffectAnimData info)
    {
        var transform = DeckBuildBattle.GetUI().popContainer.transform;
        var effect = GameAsset.Instantiate<EffectAnimObj>(transform);
        effect.data = info;
        effect.Init();
        effect.Set();
        return effect;
    }
    public void Init()
    {
        this.GetComponentSafety(out img);
        Cg().SetActive(false);
    }
    public void Set()
    {
        RectT().rotation = data.rotate;
        img.sprite = data.sprite;
        img.SetNativeSize();
    }
    public void Recycle()
    {
        DOVirtual.DelayedCall(0.1f, () => Destroy(this.gameObject));
    }
    public Sequence AOEAnim(RangeEnum baseRange)
    {
        if (!data.isAoe)
        {
            return GameAnim.Flow($"空AOEAnim");
        }
        var width = DeckBuildBattle.GetUI().popContainer.GetRectTransform().rect.width;
        switch (baseRange)
        {
            case RangeEnum.All:
                RectT().anchoredPosition = new Vector2(-width / 2 + width / 2, 0);
                break;
            case RangeEnum.Mate:
                RectT().anchoredPosition = new Vector2(-width / 2 + width / 5, 0);
                break;
            case RangeEnum.Rival:
                RectT().anchoredPosition = new Vector2(-width / 2 + width / 5 * 4, 0);
                break;
            case RangeEnum.Self:
                RectT().anchoredPosition = new Vector2(-width / 2 + width / 5, 0);
                break;
        }
        var seq = GameAnim.Flow($"全屏技能特效");

        var doActive = Cg().DOFade(1, data.duration / 2);
        var doFade = Cg().DOFade(0, data.duration / 2).Pause();
        doFade.onComplete += () => Recycle();
        
        seq.Append(doActive);
        seq.onComplete += () => doFade.Play();

        return seq;
    }
    public Sequence TargetAnim(BattleUnit target)
    {
        if (data.isAoe)
        {
            return GameAnim.AutoPlay($"空TargetAnim");
        }
        RectT().anchoredPosition = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(target).RectT().anchoredPosition;
        var seq = GameAnim.AutoPlay($"单体技能特效:{target.data.id}|{data.duration}");

        var doActive = Cg().DOFade(1, data.duration / 2);
        var doFade = Cg().DOFade(0, data.duration / 2).Pause();
        doFade.onComplete += () => Recycle();

        seq.Append(doActive);
        seq.onComplete += () => doFade.Play();
        return seq;
    }
}
