using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
namespace Like.DrawAnim
{
    public class DrawColorAnim : Anim, IDrawAnim
    {
        public DrawKey drawKey;
        public bool skipStart = false;
        public Color start = Color.white;
        public Color end = Color.white;
        public float duration = 0.5f;
        public Ease ease = Ease.Linear;
        public override async Task Play(IAnimManager manager)
        {
            var box = manager.DrawBox(drawKey);
            if (duration == 0)
            {
                box.Image.color = end;
            }
            else
            {
                if (!skipStart)
                    box.Image.color = start;
                await box.Image.DOColor(end, duration).SetEase(ease).AsyncWaitForCompletion();
            }
        }
    }
}
