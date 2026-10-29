using UnityEngine.EventSystems;

internal class BattleCardPointExit : IPointerExitHandler
{
    BattleCardUI cardObj;
    BattleCardObjAnim anime;
    BattleCardPointExit() { }
    public BattleCardPointExit(BattleCardUI cardObj)
    {
        this.cardObj = cardObj;
        anime = BattleCardUI.Anim;
    }
    public void OnPointerExit(PointerEventData eventData)
    {

    }
}