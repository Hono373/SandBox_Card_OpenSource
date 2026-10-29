using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleRoundEndBtn : MonoBehaviour, IButton
{
    Button button;
    UIGroup uiGroup;
    [SerializeReference] UIAnim enterAnime;
    [SerializeReference] UIAnim exitAnime;
    public void Init(DeckBuildBattle ddb)
    {
        this.GetComponentSafety(out button);
        UIGroup.Create(this, out uiGroup);
        button.onClick.AddListener(OnClick);
        SetActive(false);
    }
    private void OnClick()
    {
        SetActive(false);
        var currentPhase = DeckBuildBattle.Active.phase.current;
        if (currentPhase is PlayerPhase playerPhase)
        {
            new PressPlayerRoundEnd().Start(playerPhase);
        }
        else
        {
            Debug.LogWarning($"[currentPhase] is {currentPhase.GetType().Name}");
        }
    }
    public void SetInteractable(bool v)
    {
        button.interactable = v;
    }
    public Sequence Enter()
    {
        SetActive(true);
        return GameAnim.AutoPlay(nameof(Enter));
    }
    public Sequence Exit()
    {
        SetActive(false);
        return GameAnim.AutoPlay(nameof(Exit));
    }
    public void SetActive(bool v)
    {
        uiGroup.SetActive(v);
    }
}
