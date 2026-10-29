using DG.Tweening;
using UnityEngine;

public class StoryObj : MonoBehaviour
{
    [SerializeReference] public UIAnim enterAnim;
    [SerializeReference] public UIAnim leaveAnim;

    [HideInInspector] public StoryBackground background;
    [HideInInspector] public StoryDrawContainer drawContainer;
    [HideInInspector] public StoryDialogContainer dialogContainer;
    [HideInInspector] public StoryOptionContainer optionContainer;
    [HideInInspector] public StoryFuncContainer funcContainer;

    [HideInInspector] public int result;
    public UIGroup uiGroup;

    public void Init()
    {
        this.CheckInChild(out background).Init(this);
        this.CheckInChild(out drawContainer).Init(this);
        this.CheckInChild(out dialogContainer).Init(this);
        this.CheckInChild(out optionContainer).Init(this);
        this.CheckInChild(out funcContainer).Init(this);

        UIGroup.Create(this, out uiGroup);
    }

    public Sequence Exit()
    {
        uiGroup.SetActive(false);
        return GameAnim.AutoPlay();
    }

    public void SetActive(bool v)
    {
        uiGroup.SetActive(v);
    }
}
