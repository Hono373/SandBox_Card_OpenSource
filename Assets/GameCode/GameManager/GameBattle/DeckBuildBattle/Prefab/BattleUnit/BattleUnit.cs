using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BattleUnit : IBattleUnit
{
    [JsonIgnore] BattleUnitUI obj;
    public BattleUnitData data;
    public CampModule camp;
    [SerializeReference] public BattleUnitSkill unitSkill;
    /// <summary>历史意图：按行动顺序记录每次实际出招的意图快照。每轮刷新会 new 出新的克隆体覆盖 unitSkill，这里存的旧引用不受影响。</summary>
    public List<BattleUnitSkill> skillHistory = new();
    BattleUnit() { }
    [JsonIgnore] public BattleUnitCurrent current;
    public static BattleUnit Create(Unit unitData, CampEnum team)
    {
        var instance = new BattleUnit();
        instance.camp = new CampModule(team);
        instance.data = BattleUnitData.Create(unitData, team);
        instance.current = BattleUnitCurrent.Create(instance);
        // 2026-10-21 意图不在这里算：本方法由 DeckBuildBattleData.Create 在建表途中调用，
        // 此时 DeckBuildBattle.Active.data 还没赋值，任何依赖战场状态的条件（存活数、历史）都会 NPE。
        // 意图统一由 BattlePhaseStart 的 RefreshUnitIntentionGroupFlow 在数据就绪后刷新。
        return instance;
    }
    /// <summary>身上是否有指定 key 的 modifier（壁垒、腐化这类靠「存在」生效的 buff 用）。</summary>
    public bool HasModifier(string key)
    {
        foreach (var modifier in data.modifiers)
        {
            if (modifier.data.key == key) return true;
        }
        return false;
    }
    public void TakeDmg(int value, out Sequence takeDmg)
    {        takeDmg = GameAnim.AutoPlay(nameof(TakeDmg));
        TakeEffect(value);
    }
    public void TakeEffect(int dmg)
    {
        data.hp = (data.hp - dmg).GreaterThanOrEqual();
    }
    public Sequence DmgEffect(int value)
    {
        return GameAnim.AutoPlay(nameof(DmgEffect));
    }
    public UnitInfo Info => UnitInfo.Get(data.key);
    /// <summary>把本轮意图记进历史（追加到末尾）。由 UnitActionFlow 在播放时点回调，不在 FlowBuild 构建期调用。</summary>
    public void RecordSkillHistory() => skillHistory.Add(unitSkill);
    /// <summary>
    /// 播放时点刷新自己的意图：重算 unitSkill（必须在本轮 RecordSkillHistory 之后，否则条件少读一条历史），
    /// 再复用 BattleUnitIntentionBarUI 的对象池刷 UI。逻辑统一在 RefreshUnitIntentionFlow.RefreshNow。
    /// </summary>
    public void RefreshIntention() => RefreshUnitIntentionFlow.RefreshNow(this);
    public float deadTime = 1f;
}
