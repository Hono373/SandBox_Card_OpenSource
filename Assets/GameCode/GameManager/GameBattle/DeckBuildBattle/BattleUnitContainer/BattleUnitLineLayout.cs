using System.Collections.Generic;
using UnityEngine;

public class BattleUnitLineLayout
{
    public int xOffset = 200;
    public int yOffset = 200;
    public int offset = 200;
    public void SetBattleUnitPos()
    {
        var battleData = DeckBuildBattle.Active;
        SetPlayerMate(battleData.data.GetPlayer(), new());
        SetEnemy(battleData.data.GetEnemyList());
    }
    void SetPlayerMate(List<BattleUnit> player, List<BattleUnit> mates)
    {
        var list = new List<BattleUnit>();
        list.AddRange(player);
        list.AddRange(mates);

        var container = DeckBuildBattle.GetUI().unitContainer;
        for (int i = 0; i < list.Count; i++)
        {
            var obj = container.GetUnitUI(list[i]);
            switch (i)
            {
                case 0:
                    obj.RectT().anchoredPosition = new Vector2(-xOffset, yOffset);
                    break;
                case 1:
                    obj.RectT().anchoredPosition = new Vector2(-xOffset - offset, yOffset);
                    break;
                case 2:
                    obj.RectT().anchoredPosition = new Vector2(-xOffset - offset * 2, yOffset);
                    break;
                case 3:
                    obj.RectT().anchoredPosition = new Vector2(-xOffset - offset * 3 - offset, yOffset);
                    break;
            }
        }
    }
    void SetEnemy(List<BattleUnit> enemys)
    {
        var list = new List<BattleUnit>();
        list.AddRange(enemys);

        var container = DeckBuildBattle.GetUI().unitContainer;
        for (int i = 0; i < list.Count; i++)
        {
            var obj = container.GetUnitUI(list[i]);
            switch (i)
            {
                case 0:
                    obj.RectT().anchoredPosition = new Vector2(xOffset, yOffset);
                    obj.FaceTo(false);
                    break;
                case 1:
                    obj.RectT().anchoredPosition = new Vector2(xOffset + offset, yOffset);
                    obj.FaceTo(false);
                    break;
                case 2:
                    obj.RectT().anchoredPosition = new Vector2(xOffset + offset * 2, yOffset);
                    obj.FaceTo(false);
                    break;
                case 3:
                    obj.RectT().anchoredPosition = new Vector2(xOffset + offset * 3, yOffset);
                    obj.FaceTo(false);
                    break;
            }
        }
    }
}
