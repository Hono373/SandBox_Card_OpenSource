namespace Like.DrawAnim
{
    using DG.Tweening;
    using System.Threading.Tasks;
    public class DrawMoveAnim : Anim, IDrawAnim
    {
        public DrawKey drawKey;
        public int unitOffest;
        public float duration;
        public Ease ease = Ease.Linear;
        public override async Task Play(IAnimManager manager)
        {
            var offsetX = 500;
            var box = manager.DrawBox(drawKey);
            await box.RectTransform
                .DOAnchorPosX(box.RectTransform.anchoredPosition.x + offsetX * unitOffest, duration)
                .SetEase(ease)
                .AsyncWaitForCompletion();
        }
    }
}
