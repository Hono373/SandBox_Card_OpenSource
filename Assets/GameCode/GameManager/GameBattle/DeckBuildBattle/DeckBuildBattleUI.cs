using DG.Tweening;
using UnityEngine;

public class DeckBuildBattleUI : MonoBehaviour
{
    [SerializeField] DeckBuildBattle manager;

    [HideInInspector] public BattleBackgroundContainer backgroundContainer;
    [HideInInspector] public BattleUnitContainer unitContainer;
    [HideInInspector] public BattleArrowContainer arrowContainer;
    [HideInInspector] public BattlePopValueContainer popValueContainer;
    [HideInInspector] public BattleHandContainer handContainer;
    [HideInInspector] public BattleMessageContainer messageContainer;
    [HideInInspector] public BattleUIContainer uiContainer;
    [HideInInspector] public PopContainer popContainer;
    [HideInInspector] public BattleSettleContainer settleContainer;
    InformationUI informationUI;
    CanvasGroup cg;
    public CanvasGroup CanvasGroup => cg ??= this.GetComponentSafety<CanvasGroup>(out var c);
    public InformationUI InformationUI => informationUI;
    public void Init(DeckBuildBattle manager)
    {
        this.manager = manager;
        this.CheckInChild(out backgroundContainer).Init(manager);
        this.CheckInChild(out unitContainer).Init(manager);
        this.CheckInChild(out arrowContainer).Init(manager);
        this.CheckInChild(out popValueContainer).Init(manager);
        this.CheckInChild(out handContainer).Init(manager);
        this.CheckInChild(out messageContainer).Init(manager);
        this.CheckInChild(out uiContainer).Init(manager);
        this.CheckInChild(out popContainer).Init(manager);
        this.CheckInChild(out settleContainer).Init(manager);
        informationUI = GameAsset.Instantiate<InformationUI>(popContainer.transform);
        informationUI.Init();
    }
}
