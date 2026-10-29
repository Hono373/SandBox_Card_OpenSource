using UnityEngine;
using UnityEngine.EventSystems;
public class BattleCardHandler : MonoBehaviour,
    IPointerEnterHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler,
    IDragHandler,
    IBeginDragHandler,
    IEndDragHandler
{
    BattleCardUI obj;
    public void Init(BattleCardUI obj) => this.obj = obj;
    public void OnPointerEnter(PointerEventData eventData)
    {
        obj.manager.modified.interactiveMode.OnPointerEnter(obj, eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        obj.manager.modified.interactiveMode.OnPointerDown(obj, eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        obj.manager.modified.interactiveMode.Drag(obj, eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        obj.manager.modified.interactiveMode.OnPointerUp(obj, eventData);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        obj.manager.modified.interactiveMode.OnPointerExit(obj, eventData);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        obj.manager.modified.interactiveMode.OnBeginDrag(obj, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        obj.manager.modified.interactiveMode.OnEndDrag(obj, eventData);
    }
}
