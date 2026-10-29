using System.Threading.Tasks;
namespace Like.DrawAnim
{
    public class DrawDelayAnim : Anim, IDrawAnim
    {
        public float delay = 0.75f;
        public override async Task Play(IAnimManager manager)
        {
            await Task.Delay((int)(delay * 1000));
        }
    }
}