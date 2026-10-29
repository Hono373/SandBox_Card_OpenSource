using System.Threading.Tasks;
using UnityEngine;
namespace Like.DrawAnim
{
    using DG.Tweening;
    public class DrawLeaveAnim : Anim, IDrawAnim
    {
        public DrawKey drawKey = DrawKey.None;
        public override async Task Play(IAnimManager manager)
        {
            DrawColorAnim simpleColorAnim = new()
            {
                drawKey = drawKey,
                skipStart = true,
                end = Color.black,
                duration = 0.5f,
                ease = Ease.Linear
            };
            await simpleColorAnim.Play(manager);

            manager.BoxCleaner(drawKey);

            await Task.CompletedTask;
        }
    }
}