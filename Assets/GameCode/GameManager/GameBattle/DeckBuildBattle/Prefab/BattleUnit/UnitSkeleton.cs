using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;

public class UnitSkeleton : MonoBehaviour
{
    [SpineAnimation] public string idle;
    [SpineAnimation] public string attack;
    [SpineAnimation] public string hurt;
    [SpineAnimation] public string defeat;

    [HideInInspector] public SkeletonGraphic graphic;

    [SpineEvent] public string spineEvent;
    internal void Init()
    {
        this.GetComponentSafety(out graphic);
    }
    // 2026-10-20 换骨架后 Initialize(true) 会重建 AnimationState，之前播的动画全部作废。
    // 原流程是 BattleUnitUI.LoadData 先播了一次（那时还是 prefab 默认骨架），换完骨架没人重播，
    // 只剩 prefab 上写死的 startingAnimation 在起作用。把「换骨架 + 重播 idle」收在骨架组件里，避免再分两处。
    public void SetSkeletonData(SkeletonDataAsset data)
    {
        graphic.skeletonDataAsset = data;
        graphic.Initialize(true);
        Set(idle, true);
    }
    public TrackEntry Set(string anime, bool loop)
    {
        return graphic.AnimationState.SetAnimation(0, Resolve(anime), loop);
    }
    // 2026-10-20 骨架动画命名大小写不统一（angryGremlin 是 idle、fungi/champ/timeEater 是 Idle），
    // 而 Spine 按名字精确匹配（区分大小写），直接把配置名传给 SetAnimation 会让大写派骨架报
    // "Animation not found" 后静止在 setup pose。这里按 IgnoreCase 取骨架里的真实动画名；
    // 骨架里确实没有时原样返回，由 Spine 自己报错，不静默兜底。
    string Resolve(string anime)
    {
        foreach (var animation in graphic.Skeleton.Data.Animations)
        {
            if (string.Equals(animation.Name, anime, System.StringComparison.OrdinalIgnoreCase))
            { return animation.Name; }
        }
        return anime;
    }
    public void Face(bool toRight)
    {
        if (toRight)
        { graphic.Skeleton.ScaleX = 1; }
        else
        { graphic.Skeleton.ScaleX = -1; }
    }
    float underAttackTime = 0.2f;
    public void UnderAttackAnim(out Sequence underAttack)
    {
        underAttack = GameAnim.AutoPlay(nameof(UnderAttackAnim));
        underAttack.Append(graphic.DOColor(Color.red, underAttackTime / 2));
        underAttack.Append(graphic.DOColor(Color.white, underAttackTime / 2));
    }
    public Color Color { set => graphic.color = value; }
}