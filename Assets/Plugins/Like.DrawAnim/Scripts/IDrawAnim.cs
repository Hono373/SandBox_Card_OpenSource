namespace Like.DrawAnim
{
    using System.Threading.Tasks;
    public interface IDrawAnim
    {
        Task Play(IAnimManager manager);
    }
}