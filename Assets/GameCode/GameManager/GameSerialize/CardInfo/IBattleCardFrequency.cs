/// <summary>
/// 卡牌频率接口 - 每回合可使用次数
/// </summary>
public interface IBattleCardFrequency
{
    int BaseFrequency { get; }
    int Frequency(BattleCard card);
}

