using System.Collections.Generic;
using UnityEngine;

public class BattleCreator : MonoBehaviour
{
    public enum Team { Mate, Enemy }

    public DeckBuildBattleInfo battleInfo;

    public List<BattleUnitUI> mates;
    public List<BattleUnitUI> enemys;

    public void SaveBattlePos(BattleUnitUI unit, Team team)
    {
        int index;
        switch (team)
        {
            case Team.Mate:
                index = mates.IndexOf(unit);
                battleInfo.mateV2[index] = GetComponent<RectTransform>().anchoredPosition;
                break;
            case Team.Enemy:
                index = enemys.IndexOf(unit);
                battleInfo.enemyV2[index] = GetComponent<RectTransform>().anchoredPosition;
                break;
        }
        GameEditor.SaveAssets(battleInfo);
    }
}
