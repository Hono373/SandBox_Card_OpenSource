using DG.Tweening;
using UnityEngine;
public class BattleCardUIMotion : BattleFlow
{
    public BattleCardUI ui;
    public MotionData data;
    public float spd;
    public BattleCardUIMotion Start(BattleCardUI ui, MotionData data, float spd)
    {
        this.ui = ui;
        this.data = data;
        this.spd = spd;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        flow.Join(ui.RectTransform().DOAnchorPos(data.pos, spd).SetEase(Ease.Linear));
        flow.Join(ui.RectTransform().DORotateQuaternion(data.rotate, spd).SetEase(Ease.Linear));
        flow.Join(ui.RectTransform().DOScale(data.scale, spd).SetEase(Ease.Linear));
    }
}