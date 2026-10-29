using DG.Tweening;
using UnityEngine;

public class BattleUnitIntentionBarUI : MonoBehaviour
{
    /// <summary>浮动幅度（像素）</summary>
    const float FloatAmplitude = 8f;
    /// <summary>浮动角速度（弧度/秒）</summary>
    const float FloatSpeed = 2f;
    CanvasGroup cg;
    RectTransform rectT;
    Vector2 originPos;
    public CanvasGroup CG => cg ??= this.GetCanvasGroup();
    public Vector2 OriginPosition => originPos;
    /// <summary>意图 UI 对象池：Refresh 收集复用，池内剩余在消费后销毁。</summary>
    GameUIPool<BattleUnitIntentionUI> pool;
    GameUIPool<BattleUnitIntentionUI> Pool => pool ??= new(transform);
    public void Init()
    {
        rectT = this.GetRectTransform();
    }
    void Start()
    {
        originPos = rectT.anchoredPosition;
    }
    void Update()
    {
        rectT.anchoredPosition = originPos + new Vector2(0, Mathf.Sin(Time.time * FloatSpeed) * FloatAmplitude);
    }
    public void Open() => CG.SetActive(true);
    public void Close() => CG.SetActive(false);
    public Sequence Refresh(BattleUnit unit)
    {
        var seq = GameAnim.AutoPlay(nameof(Refresh));
        seq.PrependCallback(() => InstantiateObj(unit));
        return seq;
    }
    void InstantiateObj(BattleUnit unit)
    {
        // 2026-10-21 Collect 必须和 Get/DestroyExtra 在同一个回调里完成。原先 Collect 放在 Refresh（构建期），
        // 而 Get 在 PrependCallback（下一帧），同一帧刷两次时同一个 BattleUnitIntentionUI 会被重复入池，
        // DestroyExtra 销毁拖重复条目后，第二次 Get 拿到已销毁对象（DOTween: object has been destroyed）。
        Pool.Collect();
        var intentionUI = Pool.Get();
        intentionUI.Init();
        intentionUI.Refresh(unit);
        Pool.DestroyExtra();
    }
}
