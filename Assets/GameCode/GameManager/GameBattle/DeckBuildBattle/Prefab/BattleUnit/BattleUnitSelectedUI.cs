using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUnitSelectedUI : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    public TextMeshProUGUI text;
    Image image;
    UIGroup uiGroup;
    [HideInInspector] public BattleUnitUI battleUnitObj;
    [SerializeReference] UIAnim enter;
    [SerializeReference] UIAnim leave;
    public void Init(BattleUnitUI battleUnitObj)
    {
        UIGroup.Create(this, out uiGroup);
        this.GetComponentSafety(out image);
        this.CheckInChild(out text);

        text.text = battleUnitObj.manager.Info.unitName;
        this.battleUnitObj = battleUnitObj;

        uiGroup.Alpha(false);
        uiGroup.Ray(true);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        GameEditor.PingObject(eventData.pointerCurrentRaycast.gameObject);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        enter.Tween(uiGroup);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        leave.Tween(uiGroup);
    }
    public BattleUnit GetUnit()
    {
        return battleUnitObj.manager;
    }
}
