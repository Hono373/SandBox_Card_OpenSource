using UnityEngine;
using UnityEngine.UI;
public class StoryBackground : MonoBehaviour
{
    UIGroup uiGroup;
    Image image;
    internal void Init(StoryObj container)
    {
        this.GetComponentSafety(out image);
        UIGroup.Create(this, out uiGroup);
    }
    internal void Fresh(StartInfo info)
    {
        image.sprite = info.background;
    }
    internal void Fresh(DialogueInfo info)
    {
        //if (Top().backgroundContainer != null)
        //    none.Image = Top().backgroundContainer;
    }
    internal void Fresh(StoryInfo info)
    {
        //if (Top().backgroundContainer != null)
        //    none.Image = Top().backgroundContainer;
    }
    public Sprite Sprite
    {
        set => image.sprite = value;
        get => image.sprite;
    }
}