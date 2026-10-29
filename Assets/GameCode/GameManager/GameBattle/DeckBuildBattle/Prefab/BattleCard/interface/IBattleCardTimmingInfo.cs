using DG.Tweening;
using System.Collections.Generic;

public interface IBattleCardTimmingInfo
{
    void Apply(BattleCard card, List<BattleUnit> list, Sequence sequence);
    string Desc(BattleCard card);
    string ChoiceDesc(BattleCard card, BattleUnit target);
    void AfterUse(BattleCard card);
    IBattleCardTimmingInfo CreateData();
}
