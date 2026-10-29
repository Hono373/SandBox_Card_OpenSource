using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 战斗事件基类：所有「某个动作发生之后」的事件都继承它。
/// 约定与 OnPhaseStartModify 一致 —— 事件携带上下文实例，订阅者按条件匹配，
/// 需要产生技能结算的订阅者把 SkillApplyFlow 塞进 skillApplyFlows，由调用方统一接走。
///
/// 为什么要有基类：事件数量会持续增长（消耗/获得格挡/失去生命/抽到状态牌/打出攻击牌…），
/// 每个都重复写 owner + skillApplyFlows + AppendTo/PlayFlows 是纯噪音。
/// </summary>
public abstract class BattleSignal : GameModify
{
    /// <summary>触发这次事件的单位（buff 据此判断该不该响应）。</summary>
    public BattleUnit owner;
    /// <summary>订阅者提交的技能结算流。</summary>
    public List<SkillApplyFlow> skillApplyFlows = new();

    /// <summary>把订阅者产生的技能接进调用方的序列。</summary>
    public void AppendTo(Sequence sequence)
    {
        foreach (var skillApplyFlow in skillApplyFlows)
        {
            sequence.Append(skillApplyFlow.flow);
        }
    }
    /// <summary>没有宿主序列时（纯数据落堆点）独立播放。</summary>
    public void PlayFlows()
    {
        foreach (var skillApplyFlow in skillApplyFlows)
        {
            skillApplyFlow.flow.Play();
        }
    }
    protected void Broadcast(bool needSettle)
    {
        ModifyAndCreateResult(needSettle);
        // 事件本身就是结算点，没有后续消费方
        Settle();
    }
}

/// <summary>消耗事件：一张牌进入消耗堆（PileEnum.Removed）之后广播。</summary>
public class AfterExhaust : BattleSignal
{
    /// <summary>被消耗的那张牌。</summary>
    public BattleCard card;

    public AfterExhaust Start(BattleUnit owner, BattleCard card, bool needSettle)
    {
        this.owner = owner;
        this.card = card;
        Broadcast(needSettle);
        return this;
    }
}

/// <summary>获得格挡之后广播（势不可当订阅）。</summary>
public class AfterGainShield : BattleSignal
{
    /// <summary>本次获得的格挡量。</summary>
    public int amount;

    public AfterGainShield Start(BattleUnit owner, int amount, bool needSettle)
    {
        this.owner = owner;
        this.amount = amount;
        Broadcast(needSettle);
        return this;
    }
}

/// <summary>失去生命之后广播（撕裂订阅）。注意：走的是「无视护盾直接扣血」的路径才发。</summary>
public class AfterLoseHp : BattleSignal
{
    /// <summary>本次失去的生命。</summary>
    public int amount;

    public AfterLoseHp Start(BattleUnit owner, int amount, bool needSettle)
    {
        this.owner = owner;
        this.amount = amount;
        Broadcast(needSettle);
        return this;
    }
}

/// <summary>抽到状态牌 / 诅咒牌之后广播（进化、火焰吐息订阅）。</summary>
public class AfterDrawStatus : BattleSignal
{
    /// <summary>抽到的那张牌。</summary>
    public BattleCard card;

    public AfterDrawStatus Start(BattleUnit owner, BattleCard card, bool needSettle)
    {
        this.owner = owner;
        this.card = card;
        Broadcast(needSettle);
        return this;
    }
}

/// <summary>打出一张攻击牌之后广播（狂怒订阅）。</summary>
public class AfterPlayAttack : BattleSignal
{
    /// <summary>打出的那张牌。</summary>
    public BattleCard card;

    public AfterPlayAttack Start(BattleUnit owner, BattleCard card, bool needSettle)
    {
        this.owner = owner;
        this.card = card;
        Broadcast(needSettle);
        return this;
    }
}
