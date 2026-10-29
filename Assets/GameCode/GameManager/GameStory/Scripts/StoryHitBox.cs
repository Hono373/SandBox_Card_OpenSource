using UnityEngine;
using UnityEngine.EventSystems;
public class StoryHitBox : MonoBehaviour, IPointerClickHandler
{
    bool isInit = false;
    StoryObj storyManager;
    UIGroup uiGroup;
    public void Init(StoryObj manager)
    {
        storyManager = manager;
        UIGroup.Create(this, out uiGroup);

        uiGroup.Ray(false);

        Interactable = false;
        isInit = true;
    }
    public bool Interactable
    {
        get => uiGroup.GetBlocksRaycasts();
        set => uiGroup.Ray(value);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInit) return;
        storyManager.dialogContainer.GoNext();
    }
}