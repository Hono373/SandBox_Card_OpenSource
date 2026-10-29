using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDiscardPileCheckBtn : MonoBehaviour, IButton
{
    [SerializeReference] UIAnim enterAnime;
    [SerializeReference] UIAnim exitAnime;
    /// <summary>2026-10-23 弃牌堆数量文字（prefab 里 DiscardCheck/text）。</summary>
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
    /// <summary>2026-10-23 弃牌堆计数：每次都从 deck 重读真实数量，不维护本地缓存。</summary>
    public void Refresh()
    {
        text.text = DeckBuildBattle.Active.deck.GetPile(PileEnum.Discard).Count().ToString();
    }
    private void OnClick()
    {
        var pop = DeckBuildBattle.GetUI().popContainer.Get<PileCheck>();
        pop.Init();
        pop.Enter();
        pop.Fresh(DeckBuildBattle.Active.deck.GetPile(PileEnum.Discard));
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
