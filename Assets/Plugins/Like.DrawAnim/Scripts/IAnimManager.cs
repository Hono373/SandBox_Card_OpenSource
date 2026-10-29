using UnityEngine;

namespace Like.DrawAnim
{
    public interface IAnimManager
    {
        IStoryDrawBox DrawBox(DrawKey key);
        void BoxCleaner(DrawKey key);
        Sprite Background { get; set; }
    }
}