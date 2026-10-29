using System;
using DG.Tweening;
using Newtonsoft.Json;

/// <summary>
/// 阶段推进：当前阶段流程播完后启动指定阶段。
/// 单独成类而不是直接挂 onComplete，是为了走 BattleFlow.Init 的单点闸门——
/// 战斗结束时 FlowBuild 不执行，阶段链自然终止，不会推进到下一阶段。
/// 普通阶段传 NextPhase；决策阶段（FirstMoveDecide / NextMoveDecide）传决策结果。
/// </summary>
[Serializable]
public class PhaseAdvanceFlow : BattleFlow
{
    public BattlePhase nextPhase;

    /// <summary>为 true 时先等 loading 遮罩退场再启动，目前只有 BattlePhaseStart 需要。</summary>
    public bool waitLoadingExit;

    public PhaseAdvanceFlow Start(BattlePhase nextPhase)
    {
        this.nextPhase = nextPhase;
        Init();
        return this;
    }

    public PhaseAdvanceFlow StartAfterLoading(BattlePhase nextPhase)
    {
        this.nextPhase = nextPhase;
        this.waitLoadingExit = true;
        Init();
        return this;
    }

    protected override void FlowBuild()
    {
        if (waitLoadingExit)
        {
            flow.onComplete += () =>
                GameLoading.GetObj().Exit().onComplete += () => nextPhase.Start();
            return;
        }
        flow.onComplete += () => nextPhase.Start();
    }
}
