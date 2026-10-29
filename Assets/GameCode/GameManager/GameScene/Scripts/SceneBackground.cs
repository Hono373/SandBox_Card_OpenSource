using UnityEngine;
using UnityEngine.EventSystems;

public class SceneBackground : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    SceneObj sceneManager;
    internal void Init(SceneObj manager)
    {
        sceneManager = manager;
    }
    public void OnDrag(PointerEventData eventData)
    {
        sceneManager.map.Drag(eventData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        sceneManager.check.Leave();
    }
}
