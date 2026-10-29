using DG.Tweening;
using UnityEngine;
public static class BattleCardObjAnimExtensions
{
    // 这份代码作为push参考 但没有被实现
    // public static Sequence PushAnim(this BattleCardObj cardObj)
    // {
    //     var process = GameSeq.Create(nameof(CardPushAnim));
    //     var handList = DeckBuildBattle.Active.deck.GetPile(DBBDeck.PileEnum.Hand).list;

    //     foreach (var otherCard in handList)
    //     {
    //         var obj = otherCard.GetObj();
    //         if (obj.manager.Interactive.stateEnum == CardStateEnum.Idle) continue;
    //         if (otherCard == cardObj.manager) continue;
    //         bool isGreater = handList.IndexOf(otherCard) > handList.IndexOf(cardObj.manager);
    //         var pos = obj.argData.pos - new Vector2(BattleCardObj.Anim.push.pushOffset, 0) * IsGreater(isGreater);

    //         var push = obj.Motion(pos, obj.argData.rotate, 1, BattleCardObj.Anim.push.pushSpd);
    //     }

    //     return process;

    //     int IsGreater(bool v)
    //     {
    //         if (v)
    //             return 1;
    //         else
    //             return -1;

    //     }
    // }
    public static Sequence IdleAnim(this BattleCardUI obj)
    {
        return obj.Motion(obj.manager.argData.pos, obj.manager.argData.rotate, Vector3.one , BattleCardUI.Anim.idle.spd, nameof(IdleAnim));
    }

    public static Sequence Motion(this BattleCardUI ui, Vector2 pos, Quaternion rotate, Vector3 scale, float spd, string name = "Motion")
    {
        var cardMotion = GameAnim.AutoPlay(name);

        var doMove = DoAnchorPos(ui, pos, spd);
        var doRotate = DoRotate(ui, rotate, spd);
        var doScale = DoScale(ui, scale, spd);

        cardMotion.Join(doRotate);
        cardMotion.Join(doMove);
        cardMotion.Join(doScale);

        return cardMotion;
    }
    public static Tween DoAnchorPos(this BattleCardUI obj, Vector2 pos, float animSpd)
    {
        return obj.RectTransform().DOAnchorPos(pos, animSpd).SetEase(Ease.Linear);
    }
    public static Tween DoRotate(this BattleCardUI obj, Quaternion quaternion, float animSpd)
    {
        return obj.RectTransform().DORotateQuaternion(quaternion, animSpd).SetEase(Ease.Linear);
    }
    public static Tween DoScale(this BattleCardUI obj, float value, float animSpd)
    {
        return obj.RectTransform().DOScale(value, animSpd).SetEase(Ease.Linear);
    }
    public static Tween DoScale(this BattleCardUI obj, Vector3 scale, float animSpd)
    {
        return obj.RectTransform().DOScale(scale, animSpd).SetEase(Ease.Linear);
    }
    public static Tween DoColor(this BattleCardUI obj, Color color, float animSpd)
    {
        return obj.bg.DOColor(color, animSpd).SetEase(Ease.Linear);
    }
}
