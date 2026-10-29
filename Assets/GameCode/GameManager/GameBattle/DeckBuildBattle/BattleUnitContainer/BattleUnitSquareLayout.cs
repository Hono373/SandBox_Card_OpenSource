using System.Collections.Generic;
using UnityEngine;

public class BattleUnitSquareLayout
{
    public int xOffset = 200;
    public int lineSpace = 200;
    public int columnSpace = 200;
    public int yOffset = 200;
    public int offset = 200;
    public void SetBattleUnitPos()
    {
        var battle = DeckBuildBattle.Active;
        SetPlayerMate(battle.data.GetPlayer(), new());
        SetEnemy(battle.data.GetEnemyList());
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
                    obj.RectT().anchoredPosition = new Vector2(-xOffset - offset, yOffset - lineSpace);
                    break;
                case 2:
                    obj.RectT().anchoredPosition = new Vector2(-xOffset - columnSpace, yOffset);
                    break;
                case 3:
                    obj.RectT().anchoredPosition = new Vector2(-xOffset - columnSpace - offset, yOffset - lineSpace);
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
                    obj.RectT().anchoredPosition = new Vector2(xOffset + offset, yOffset - lineSpace);
                    obj.FaceTo(false);
                    break;
                case 2:
                    obj.RectT().anchoredPosition = new Vector2(xOffset + columnSpace, yOffset);
                    obj.FaceTo(false);
                    break;
                case 3:
                    obj.RectT().anchoredPosition = new Vector2(xOffset + columnSpace + offset, yOffset - lineSpace);
                    obj.FaceTo(false);
                    break;
            }
        }
    }
}
