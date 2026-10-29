using UnityEngine;
using UnityEngine.UI;

namespace Like.DrawAnim
{
    public interface IStoryDrawBox
    {
        RectTransform RectTransform { get; }
        Sprite StoryBackground { get; }
        Image Image { get; }
        void SetActive(bool v);
    }
}