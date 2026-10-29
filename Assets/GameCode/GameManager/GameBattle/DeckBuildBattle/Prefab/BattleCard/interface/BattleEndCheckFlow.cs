using System;
using System.Collections.Generic;
using DG.Tweening;
public class BattleEndCheckFlow : BattleFlow
{
    public List<DeathCheckModify> unitDeathCheckModifys = new();
    public RefreshBattleCardGroupFlow refreshBattleCardGroupFlow = new();
    public List<BattleUnit> deathUnits = new();
    public BattleEndCheckFlow Start()
    {
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        foreach (var unit in DeckBuildBattle.Active.data.unitDict.GetAll().Values)
        {
            unitDeathCheckModifys.Add(new DeathCheckModify().Start(this, unit, true));
        }
        if (deathUnits.Count != 0)
        {
            foreach (var unit in deathUnits)
            {
                var deathSkillPhase = new DeathSkillPhase().Start(this, unit);
                flow.Join(deathSkillPhase.flow);
            }
            flow.Append(refreshBattleCardGroupFlow.Start(needSettle: false).flow);
            var loop = new BattleEndCheckFlow().Start();
            flow.Append(loop.flow);
        }
        else
        {
            var winnerCheckPhase = new WinnerCheckPhase().Start(this);
            flow.Append(winnerCheckPhase.flow);
        }
    }
}