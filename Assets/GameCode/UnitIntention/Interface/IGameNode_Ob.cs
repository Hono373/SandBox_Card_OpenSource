using System.Collections.Generic;

public interface IGameNode_Ob
{
    bool IsEnd { get; }

    int Weight { get; }
    IReadOnlyList<int> ChildsWeights();
    ConditionBox ConditionBox { get; }
    IReadOnlyList<IGameNode_Ob> Childs { get; }

    int? GetChildIndex(BattleUnit unit);
}
