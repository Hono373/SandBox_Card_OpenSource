using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
[Serializable]
public class BattleTiming
{
    public TimingEnum timingEnum;
    [SerializeReference] public PileEnum pileEnum = PileEnum.Discard;
    public List<Skill> skills = new();
    internal void ValidateData()
    {
        if (skills == null || skills.Count == 0)
            Debug.LogError($"卡牌 {timingEnum} skillAtoms 为空");
        foreach (var skillAtom in skills)
        {
            if (skillAtom == null || skillAtom.GetType().IsAbstract)
                Debug.LogError($"卡牌 {timingEnum} 存在无效 skillAtom（null 或抽象类）");
            else
                skillAtom.ValidateData();
        }

    }
    public void AfterUse(BattleCard card)
    {
        var deck = DeckBuildBattle.Active.deck;

        // 能力牌常驻：不进弃牌/消耗堆，登记到 battle.powers，
        // 之后由 CardPhaseTimingFlow 在相位切换时按 timingEnum 触发。
        // 2026-10-22 同时报账到 Destroy 堆（移出游戏）：powers 只负责触发，牌堆记账靠 Destroy，
        // 才真正实现「彻底出局」。注意别放 Removed —— 那是消耗堆，属于可被发掘取回的池子。
        var info = card?.modified?.Info;
        if (info != null && info.graphic != null && info.graphic.type == TypeEnum.Ability)
        {
            DeckBuildBattle.Active.AddPower(card);
            deck.GetPile(PileEnum.Destroy).Add(card);
            return;
        }

        // 2026-10-22 腐化：技能牌打出后直接进消耗堆（同样不能读 card.modified.owner）
        if (info.graphic.type == TypeEnum.Skill && BattleCardModify_CostRule.MateHasCorruption())
        {
            deck.GetPile(PileEnum.Removed).Add(card);
            new AfterExhaust().Start(card.modified.owner, card, true).PlayFlows();
            return;
        }

        deck.GetPile(pileEnum).Add(card);

        // 2026-10-22 消耗事件：这张牌进的是消耗堆时才广播（黑暗之拥 / 无惧疼痛等订阅）
        if (pileEnum == PileEnum.Removed)
            new AfterExhaust().Start(card.modified.owner, card, true).PlayFlows();
    }
    public string Desc()
    {
        var list = new List<string>();
        foreach (var skill in skills)
        {
            list.Add(skill.Desc());
        }
        return string.Join(", ", list);
    }
    public BattleTiming Create()
    {
        var instance = new BattleTiming() { timingEnum = timingEnum, pileEnum = pileEnum };
        foreach (var skillAtom in skills)
        {
            instance.skills.Add(skillAtom.Create());
        }
        return instance;
    }
}
