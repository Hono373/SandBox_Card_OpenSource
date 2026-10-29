using UnityEngine;
public class BattleMessageContainer : MonoBehaviour
{
    BattleMessage message;
    public void Init(DeckBuildBattle manager)
    {
        this.CheckInChild(out message).Init();
    }
    public void Message(string msg)
    {
        message.Message(msg);
    }
}