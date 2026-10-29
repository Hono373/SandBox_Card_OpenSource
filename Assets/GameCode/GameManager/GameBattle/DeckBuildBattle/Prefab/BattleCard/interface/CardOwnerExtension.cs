public static class CardOwnerExtension
{
    public static BattleUnit GetOwnerBattleUnit(this BattleCard card)
    {
        var ownerId = card.data.owner;
        if (string.IsNullOrEmpty(ownerId))
            return null;
        return DeckBuildBattle.Active.data.unitDict.GetBattleUnitById((string)ownerId);
    }
}
