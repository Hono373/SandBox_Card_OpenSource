using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDrawPileCheckBtn : MonoBehaviour, IButton
{
    [SerializeReference] UIAnim enterAnime;
    [SerializeReference] UIAnim exitAnime;
    [HideInInspector] public TextMeshProUGUI text;
    Button button;
    UIGroup uiGroup;
    public void Init(DeckBuildBattle ddb)
    {
        this.GetComponentSafety(out button);
        UIGroup.Create(this, out uiGroup);
        button.onClick.AddListener(OnClick);
        this.CheckInChild(out text);
        Refresh();
        SetActive(true);
    }
    /// <summary>2026-10-23 抽牌堆计数：每次都从 deck 重读真实数量，不维护本地缓存。</summary>
    public void Refresh()
    {
        text.text = DeckBuildBattle.Active.deck.GetPile(PileEnum.Draw).Count().ToString();
    }
    private void OnClick()
    {
        var pop = DeckBuildBattle.GetUI().popContainer.Get<PileCheck>();
        pop.Init();
        pop.Enter();
        pop.Fresh(DeckBuildBattle.Active.deck.GetPile(PileEnum.Draw));
    }
    public Sequence Enter()
    {
        SetActive(true);
        return GameAnim.AutoPlay(nameof(Enter)); ;
    }
    public Sequence Exit()
    {
        SetActive(false);
        return GameAnim.AutoPlay(nameof(Exit)); ;
    }
    public void SetActive(bool v)
    {
        uiGroup.SetActive(v);
    }
}
