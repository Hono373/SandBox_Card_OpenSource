namespace Like.DrawAnim
{
    using DG.Tweening;
    using System.Threading.Tasks;
    using UnityEngine;
    public class DrawJumpAnim : Anim, IDrawAnim
    {
        public DrawKey drawKey;
        public int unitOffset;
        public float jumpPower = 1f;
        public int jumpNumb = 1;
        public float duration = 1f;
        public Ease ease = Ease.Linear;
        public override async Task Play(IAnimManager manager)
        {
            var offset = 500;
            var box = manager.DrawBox(drawKey);
            var target = new Vector2(box.RectTransform.anchoredPosition.x + unitOffset * offset, box.RectTransform.anchoredPosition.y);
            await box.RectTransform.DOJumpAnchorPos(
                  target,
                  jumpPower * 100,
                  jumpNumb,
                  duration
                  )
                  .SetEase(ease)
                  .AsyncWaitForCompletion();
        }
        public async Task UnUsed(IStoryDrawBox standing)
        {
            var offset = 500;
            var startPos = standing.RectTransform.anchoredPosition;
            Sequence jump = DOTween.Sequence();
            Tween xTween = standing.RectTransform.XMove(unitOffset * offset, 1, duration);
            Tween yTween = standing.RectTransform.YMove(jumpPower, 1, duration).SetLoops(2, LoopType.Yoyo);
            jump.Append(xTween).Join(yTween);
            await jump.AsyncWaitForCompletion();
        }
    }
    public static class JumpAnimExtension
    {
        public static Tween XMove(this RectTransform target, float offsetX, int numJumps, float duration, bool snapping = false)
        {
            return DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(target.anchoredPosition.x + offsetX, 0), duration / numJumps)
                    .SetOptions(AxisConstraint.X, snapping)
                    .SetEase(Ease.Linear);
        }
        public static Tween YMove(this RectTransform target, float jumpPower, int numJumps, float duration, bool snapping = false)
        {
            return DOTween.To(() => target.anchoredPosition, x => target.anchoredPosition = x, new Vector2(0, jumpPower), duration / (numJumps * 2))
                .SetOptions(AxisConstraint.Y, snapping)
                .SetEase(Ease.OutQuad)
                .SetRelative();
        }
    }
}
