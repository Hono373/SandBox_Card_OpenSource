using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SceneLevel : MonoBehaviour, IDragHandler
{
    SceneObj manager;

    public UIGroup uiGroup;
    public LevelData data;

    Image image;
    UIButton btn;
    TextMeshProUGUI text;
    public void Init(SceneObj manager, LevelData data, Vector2 pos)
    {
        this.manager = manager;

        this.GetComponentSafety(out image);
        this.GetComponentSafety(out btn).Init(OnClick);
        this.CheckInChild(out text);

        UIGroup.Create(this, out uiGroup);

        uiGroup.RectT.anchoredPosition = pos;
        Fresh(data);
    }
    public void Fresh(LevelData data)
    {
        this.data = data;
        text.text = GameLanguage.GetText(data.Info.Name);
    }
    void OnClick()
    {
        if (manager.check.uiGroup.GetIsShow())
        {
            if (manager.check.Focus == data)
            {
                return;
            }
            else
            {
                manager.check.Set(data);
            }
        }
        else
        {
            manager.check.Enter();
        }
        Vector2 targetPos = GetScreenPos();
        manager.map.Focus(targetPos);
        manager.check.Set(data);
    }
    public Vector2 GetScreenPos()
    {
        var mapRect = manager.map.uiGroup.RectT;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapRect,
            new Vector2(Screen.width / 3, Screen.height / 2),
            GameCamera.GetObj().camera,
            out Vector2 screenRectPos);

        var offset = uiGroup.RectT.IsLeftAnchor(mapRect) - screenRectPos.x;
        var targetPos = mapRect.anchoredPosition - new Vector2(offset, 0);
        return targetPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        manager.map.Drag(eventData);
    }
}
