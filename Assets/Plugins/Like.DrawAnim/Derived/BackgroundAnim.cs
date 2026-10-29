namespace Like.DrawAnim
{
    using System.Threading.Tasks;
    using UnityEngine;
    public class BackgroundAnim : Anim, IDrawAnim
    {
        public Sprite background = null;
        public override async Task Play(IAnimManager manager)
        {
            manager.Background = background;
            await Task.CompletedTask;
        }
    }
}
