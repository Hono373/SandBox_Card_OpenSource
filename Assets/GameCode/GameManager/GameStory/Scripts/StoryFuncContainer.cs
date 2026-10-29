using UnityEngine;

public class StoryFuncContainer : MonoBehaviour
{
    UIGroup uiGroup;
    internal void Init(StoryObj storyContainer)
    {
        UIGroup.Create(this, out uiGroup);
        uiGroup.SetActive(false);
    }
}
