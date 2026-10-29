using System;

[Serializable]
public enum TimingEnum
{
    /// <summary>打出时结算（唯一走「打牌流程」的时机）。</summary>
    ActiveUse = 0,
    /// <summary>弃牌时。目前无消费方，配了等于空转。</summary>
    DiscardUse = 1,
    /// <summary>玩家回合开始。只对常驻的能力牌生效，见 CardPhaseTimingFlow。</summary>
    PlayerPhaseStart = 2,
    /// <summary>玩家回合结束。只对常驻的能力牌生效。</summary>
    PlayerPhaseEnd = 3,
}
