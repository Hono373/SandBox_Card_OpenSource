using DG.Tweening;
using UnityEngine;

public class LoadingObj : MonoBehaviour, IVisual
{
    [SerializeReference] UIAnim enterAnim;
    [SerializeReference] UIAnim leaveAnim;
    RectTransform rectT;
    CanvasGroup cg;
    public void Init()
    {
        this.CheckScreenSpaceCanvas(GameCamera.GetObj().camera);
        this.GetComponentSafety(out rectT);
        this.GetComponentSafety(out cg);
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }
    public Sequence Enter()
    {
        var sequence = GameAnim.AutoPlay(); ;
        sequence.PrependCallback(() => { cg.blocksRaycasts = true; cg.alpha = 0; });
        sequence.Join(enterAnim.Tween(rectT, cg));
        return sequence;
    }
    public Sequence Exit()
    {
        var sequence = GameAnim.AutoPlay(); ;
        sequence.Join(leaveAnim.Tween(rectT, cg));
        sequence.onComplete += () => cg.blocksRaycasts = false;
        return sequence;
    }
    public void SetActive(bool v)
    {
        cg.alpha = v ? 1 : 0;
        cg.blocksRaycasts = v;
    }
}

