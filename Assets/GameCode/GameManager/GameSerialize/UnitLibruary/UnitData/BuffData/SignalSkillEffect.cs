using DG.Tweening;
using System;

/// <summary>
/// 通用信号订阅器：把「某个战斗事件发生后，结算一个 Skill」抽成基类。
/// 与 PhaseSkill 完全同构（PhaseSkill 用 Skill 覆盖所有相位触发，这里用 Skill 覆盖所有战斗信号）。
///
/// 加新 buff 只需要：写一个 3 行的子类 + 一个 ModifierInfo_XXX.asset（type 指向子类、data 里配 skill），
/// 不用写任何效果代码 —— 效果全在 asset 的 Skill 里。
/// </summary>
public abstract class SignalSkillEffect<T> : ModifierEffect<T> where T : BattleSignal
{
    /// <summary>只在「事件主人 == 本 buff 主人」时触发。</summary>
    public bool ownerOnly = true;
    /// <summary>触发时结算的技能。</summary>
    public Skill skill;

    public sealed override void Modify(Modifier modifier, T context)
    {
        if (ownerOnly && context.owner != modifier.GetOwner()) return;
        context.skillApplyFlows.Add(new SkillApplyFlow().Start(skill, modifier.GetOwner(), null, 0, true));
    }
}

/// <summary>消耗时触发（黑暗之拥 / 无惧疼痛）。</summary>
[Serializable]
public class ExhaustSkillEffect : SignalSkillEffect<AfterExhaust>
{
    public override ModifierEffect Create() => new ExhaustSkillEffect { ownerOnly = ownerOnly, skill = skill.Create() };
}

/// <summary>获得格挡后触发（势不可当）。</summary>
[Serializable]
public class GainShieldSkillEffect : SignalSkillEffect<AfterGainShield>
{
    public override ModifierEffect Create() => new GainShieldSkillEffect { ownerOnly = ownerOnly, skill = skill.Create() };
}

/// <summary>失去生命后触发（撕裂）。</summary>
[Serializable]
public class LoseHpSkillEffect : SignalSkillEffect<AfterLoseHp>
{
    public override ModifierEffect Create() => new LoseHpSkillEffect { ownerOnly = ownerOnly, skill = skill.Create() };
}

/// <summary>抽到状态/诅咒牌后触发（进化 / 火焰吐息）。</summary>
[Serializable]
public class DrawStatusSkillEffect : SignalSkillEffect<AfterDrawStatus>
{
    public override ModifierEffect Create() => new DrawStatusSkillEffect { ownerOnly = ownerOnly, skill = skill.Create() };
}

/// <summary>打出攻击牌后触发（狂怒）。</summary>
[Serializable]
public class PlayAttackSkillEffect : SignalSkillEffect<AfterPlayAttack>
{
    public override ModifierEffect Create() => new PlayAttackSkillEffect { ownerOnly = ownerOnly, skill = skill.Create() };
}

/// <summary>
/// 纯标记型 buff：不订阅任何事件，只靠「存在」被别人查询（如壁垒：清格挡前查有没有它）。
/// 需要一个空实现是因为 ModifierInfo.modifierEffects 为空时 asset 不好写。
/// </summary>
[Serializable]
public class FlagEffect : ModifierEffect<AfterTakeDmg>
{
    public override ModifierEffect Create() => new FlagEffect();
    public override void Modify(Modifier modifier, AfterTakeDmg context) { }
}

/// <summary>
/// 本回合临时 buff：在指定阶段开始时把自己摘掉。
/// 用于「本回合内每打出攻击牌获得 3 点格挡」这类效果 —— 没有它，buff 会永久留在身上。
/// </summary>
[Serializable]
public class ExpireOnPhaseEffect : ModifierEffect<OnPhaseStartModify>
{
    /// <summary>在哪个阶段结束时过期（默认玩家回合结束）。</summary>
    public PhaseEnum phaseEnum = PhaseEnum.PlayerPhaseEnd;

    public override ModifierEffect Create() => new ExpireOnPhaseEffect { phaseEnum = phaseEnum };

    public override void Modify(Modifier modifier, OnPhaseStartModify context)
    {
        if (phaseEnum != PhaseEnum.PlayerPhaseEnd) return;
        if (!(context.phase is PlayerPhaseEnd)) return;

        var owner = modifier.GetOwner();
        owner.data.modifiers.Remove(modifier);
        GameModifyCenter.RemoveRegister(modifier);
        DeckBuildBattle.GetUI().unitContainer.GetUnitUI(owner).modifierBarUI.RemoveModifier(modifier);
    }
}

/// <summary>
/// 双发：本回合打出的下一张攻击牌再结算一遍。
/// 不碰 BattleCardUseFlow —— 挂在 AfterPlayAttack 事件后面重跑一次该牌的 ActiveUse 技能即可，
/// 目标同样交给 RangeRule.AIChoice。用掉一层就减一层，减到 0 自己摘掉。
/// </summary>
[Serializable]
public class RepeatAttackEffect : ModifierEffect<AfterPlayAttack>
{
    public override ModifierEffect Create() => new RepeatAttackEffect();

    public override void Modify(Modifier modifier, AfterPlayAttack context)
    {
        var owner = modifier.GetOwner();
        if (context.owner != owner) return;
        if (context.card.modified.graphic.type != TypeEnum.Attack) return;

        foreach (var timing in context.card.modified.timings)
        {
            if (timing.timingEnum != TimingEnum.ActiveUse) continue;
            foreach (var skill in timing.skills)
            {
                var copy = skill.Create();
                var choice = copy.rangeRule.AIChoice(owner);
                context.skillApplyFlows.Add(new SkillApplyFlow().Start(copy, owner, choice, 0, needSettle: true));
            }
            break;
        }

        modifier.data.layer--;
        var bar = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(owner).modifierBarUI;
        if (modifier.data.layer > 0)
        {
            bar.AddOrRefresh(modifier);
            return;
        }
        owner.data.modifiers.Remove(modifier);
        GameModifyCenter.RemoveRegister(modifier);
        bar.RemoveModifier(modifier);
    }
}

/// <summary>
/// 活动肌肉：回合结束时扣掉 `layer` 点力量（layer = 这张 buff 的层数），然后自摘。
///
/// 层数语义：**buff 的层数就是「欠的力量点数」**，不是打出次数 ——
/// 打出一次 +2 力量时同时 +2 层，两次打出就是 4 层，回合结束扣 4 点。这样不需要额外乘系数。
/// 为什么不用 PhaseSkill：它的 skill 是静态配置，扣的数值不能随层数缩放。
/// </summary>
[Serializable]
public class FlexExpireEffect : ModifierEffect<OnPhaseStartModify>
{
    public override ModifierEffect Create() => new FlexExpireEffect();

    public override void Modify(Modifier modifier, OnPhaseStartModify context)
    {
        if (!(context.phase is PlayerPhaseEnd)) return;

        var owner = modifier.GetOwner();
        var bar = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(owner).modifierBarUI;

        // 扣力量：ModifierAddFlow 会顺手刷新力量图标与敌方意图
        var add = new ModifierAdd { key = LimitBreakEffect.STRENGTH_KEY, layer = -modifier.data.layer, only = true };
        var result = add.Apply(owner, owner, null);
        new ModifierAddFlow().Start(add, owner, owner, result).flow.Play();

        // 摘掉临时力量：数据 + 全局注册 + UI 图标（三者原本只清了前两个）
        owner.data.modifiers.Remove(modifier);
        GameModifyCenter.RemoveRegister(modifier);
        bar.RemoveModifier(modifier);
    }
}
