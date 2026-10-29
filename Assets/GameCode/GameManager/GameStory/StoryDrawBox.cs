using Like.DrawAnim;
using UnityEngine;
using UnityEngine.UI;
public class StoryDrawBox : MonoBehaviour, IStoryDrawBox
{
    StoryObj storyManager;
    Image image;
    UIGroup uiGroup;
    public RectTransform RectTransform => uiGroup.RectT;
    public Sprite StoryBackground => storyManager.background.Sprite;
    public Image Image
    {
        get => image;
    }
    public StoryDrawBox Init(StoryObj container)
    {
        storyManager = container;

        this.CheckInChild(out image);

        UIGroup.Create(this, out uiGroup);
        return this;
    }
    public void SetActive(bool v)
    {
        uiGroup.SetActive(v);
    }
}
