using UnityEngine;

public class DisplayCard : MonoBehaviour, IInfiniteObject
{
    internal DisplayCardGraphic graphic;
    internal string cardKey;

    internal static DisplayCard Create(Transform transform)
    {
        var card = GameAsset.Instantiate<DisplayCard>(transform);
        card.GetComponentSafety(out card.graphic);
        card.graphic.Init();
        card.graphic.uiGroup.Ray(false);
        return card;
    }

    public void Fresh(IInfiniteData cardData)
    {
        if (cardData is BattleCard battleCard)
        {
            cardKey = battleCard.data.key;
            var info = CardInfo.Get(cardKey);
            graphic.Fresh(info);
        }
    }

    public void SetPos(Vector2 pos)
    {
        graphic.uiGroup.RectT.anchoredPosition = pos;
    }

    public void SetActive(bool v)
    {
        graphic.SetActive(v);
    }
    public Rect Rect
    {
        get => graphic.uiGroup.RectT.rect;
    }
}
