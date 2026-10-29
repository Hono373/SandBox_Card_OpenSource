using System;
using UnityEngine.EventSystems;

[Serializable]
internal class BattleCardPointEnter : IPointerEnterHandler
{
    BattleCardUI cardObj;
    BattleCardObjAnim anime;
    BattleCardPointEnter() { }
    public BattleCardPointEnter(BattleCardUI cardObj)
    {
        this.cardObj = cardObj;
        anime = BattleCardUI.Anim;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {

    }
}