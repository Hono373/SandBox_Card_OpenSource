using UnityEngine;
public class BattleSettleContainer : MonoBehaviour
{
    DeckBuildBattle manager;
    UIGroup ui;
    internal void Init(DeckBuildBattle cardBattleManager)
    {
        manager = cardBattleManager;
    }
    public void Message(BattleResultEnum result)
    {
        var settle = BattleSettle.Create(manager);

        switch (result)
        {
            case BattleResultEnum.PlayerWin:
                settle.Enter("玩家胜利");
                break;
            case BattleResultEnum.PlayerLose:
                settle.Enter("玩家失败");
                break;
            case BattleResultEnum.None:
                GameDebug.Log("BattleResult.none");
                break;
        }
    }
}