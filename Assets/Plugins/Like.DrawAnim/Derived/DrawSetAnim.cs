using System.Threading.Tasks;
using UnityEngine;
namespace Like.DrawAnim
{
    using DG.Tweening;
    public class DrawSetAnim : Anim, IDrawAnim
    {
        public DrawKey drawKey = DrawKey.None;
        public Sprite standing = null;
        public int unit = 0;
        const float offset = 500;
        public bool isComeOn = false;
        public override async Task Play(IAnimManager manager)
        {
            var box = manager.DrawBox(drawKey);
            box.Image.sprite = standing;
            box.RectTransform.anchoredPosition = new(offset * unit, 0);

            if (isComeOn)
            {
                DrawColorAnim simpleColorAnim = new()
                {
                    drawKey = drawKey,
                    skipStart = false,
                    start = Color.black,
                    end = Color.white,
                    duration = 0.5f,
                    ease = Ease.Linear
                };
                await simpleColorAnim.Play(manager);
            }
            await Task.CompletedTask;
        }
    }
}