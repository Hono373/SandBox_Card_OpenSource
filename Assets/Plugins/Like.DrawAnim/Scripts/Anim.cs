namespace Like.DrawAnim
{
    using System;
    using System.Threading.Tasks;

    [Serializable]
    public abstract class Anim : IDrawAnim
    {
        public bool parallel;
        public abstract Task Play(IAnimManager manager);
    }
}