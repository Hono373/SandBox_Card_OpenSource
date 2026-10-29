using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleMessage : MonoBehaviour
{
    [HideInInspector] public UIGroup uiGroup;
    Image bg;
    TextMeshProUGUI text;
    public float displayTime;
    [SerializeReference] UIAnim enterAnim;
    [SerializeReference] UIAnim leaveAnim;
    Sequence sequence;
    public void Init()
    {
        UIGroup.Create(this, out uiGroup);
        this.GetComponentSafety(out bg);
        this.CheckInChild(out text);

        uiGroup.SetActive(false);
    }
    public void Message(string msg)
    {
        sequence?.Kill();
        text.text = msg;
        sequence = GameAnim.AutoPlay(nameof(BattleMessage));
        var enter = enterAnim.Tween(uiGroup);
        var leave = leaveAnim.Tween(uiGroup);
        sequence.Append(enter);
        sequence.AppendInterval(displayTime);
        sequence.Append(leave);
    }
}