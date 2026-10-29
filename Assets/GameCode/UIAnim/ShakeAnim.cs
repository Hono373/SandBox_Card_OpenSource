using DG.Tweening;
using UnityEngine;
public class ShakeAnim : UIAnim
{
    public float duration = 1;
    public float strength = 100;
    public int vibrato = 10;
    public float randomness = 90;
    public bool snapping = false;
    public bool fadeOut = true;
    public ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full;
    public override Tween Tween(RectTransform rectT, CanvasGroup cg)
    {
        return rectT.
            DOShakeAnchorPos(duration, strength, vibrato, randomness, snapping, fadeOut);
    }
}
