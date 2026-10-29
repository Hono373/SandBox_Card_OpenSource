using System.Collections.Generic;
public interface ISkillInfo_Ob
{
    SelectorBox_Ob SelectMode();
    List<ISkillEffectInfo_Ob> EffectInfos();
    List<IntentionUIData_Ob> GetUIData();
    string Desc(BattleUnit source, BattleUnit target);
}


