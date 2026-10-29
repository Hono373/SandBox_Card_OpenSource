using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopValueObj : MonoBehaviour
{
    [SerializeField] Color green;
    [SerializeField] Color white;
    [SerializeField] Color red;
    public enum ValueColor
    {
        White, Red, Green, Blue
    }

    [HideInInspector] public bool inUse = false;
    [HideInInspector] public TextMeshProUGUI text;
    public UIGroup uiGroup;
    [SerializeReference] UIAnim scaleAnim;
    [SerializeReference] UIAnim leaveAnim;
    [SerializeReference] UIAnim jumpAnim;
    public static PopValueObj Create(Transform container)
    {
        var top = DeckBuildBattle.GetUI();
        var jumpValue = GameAsset.Instantiate<PopValueObj>(container);
        jumpValue.Init();
        return jumpValue;
    }
    public void Init()
    {
        UIGroup.Create(this, out uiGroup);
        this.CheckInChild(out text);
        uiGroup.SetActive(false);
    }
    public void ShowPop(BattleUnitUI unit, int value, ValueColor color, float duration, out Sequence showPop)
    {
        Color useColor = Color.yellow;
        switch (color)
        {
            case ValueColor.Red:
                useColor = red;
                break;
            case ValueColor.Green:
                useColor = green;
                break;
            case ValueColor.Blue:
                useColor = Color.blue;
                break;
        }
        if (value == 0) useColor = white;
        text.text = value.ToString();
        text.color = useColor;
        var startPos = unit.RectT().anchoredPosition;
        uiGroup.RectT.anchoredPosition = startPos;

        showPop = GameAnim.AutoPlay($"PopValue:{unit.manager.data.id}|{color}|{value}"); 
        var jumpAnim = (JumpAnim)this.jumpAnim;
        var scaleTarget = 1 + Mathf.Clamp(value / 12f, 0, 3);
        var scaleTime = Mathf.Min(duration * 0.55f, 0.2f);
        uiGroup.RectT.localScale = Vector3.one * 0.3f;
        var scale = uiGroup.RectT
            .DOScale(scaleTarget, scaleTime)
            .SetEase(Ease.OutBack, 1.3f);
        var jump = jumpAnim.DurationTween(uiGroup.RectT, uiGroup.CG, duration);
        var fadeDelay = duration * 0.35f;
        var fadeTime = duration * 0.65f;
        var leave = uiGroup.CG
            .DOFade(0, fadeTime)
            .SetDelay(fadeDelay)
            .SetEase(Ease.InCubic);
        showPop.Append(uiGroup.CG.DOFade(1, 0));
        showPop.Join(scale);
        showPop.Join(jump);
        showPop.Join(leave);
        showPop.OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.1f, () => DeckBuildBattle.GetUI().popValueContainer.Recycle(this));
        });
    }
}
