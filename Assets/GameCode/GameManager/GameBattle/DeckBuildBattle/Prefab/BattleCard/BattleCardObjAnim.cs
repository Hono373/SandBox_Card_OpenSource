using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(BattleCardObjAnim))]
public class BattleCardObjAnim : ScriptableObject
{
    public static BattleCardObjAnim Get()
    {
        return GameAsset.GetSingletonSo<BattleCardObjAnim>();
    }

    public BattleCardUI BattleCardPrefab;

    public HandDrawAnim handDraw = new();
    public HandCreateAnim handCreate = new();
    public CardArrangeCalculation handArrangeCalculation = new();
    public CardPushAnim push = new();
    public CardDestroyAnim destroy = new();

    public CardIdleAnim idle = new();
    public CardFocusAnimInfo focus = new();

    public CardChoiceAnimInfo choice = new();
    public CardHoldAnim hold = new();

    public CardShowAnime show = new();
    public CardHideAnim hide = new();

    public float cardHeight => BattleCardPrefab.transform.GetComponent<RectTransform>().rect.height;
    public float cardWidth => BattleCardPrefab.transform.GetComponent<RectTransform>().rect.width;

    [Button("手牌排序")]
    void Test()
    {
        DeckBuildBattle.GetUI().handContainer.CalculateArg();
        DeckBuildBattle.GetUI().handContainer.Arrange();
    }
}
