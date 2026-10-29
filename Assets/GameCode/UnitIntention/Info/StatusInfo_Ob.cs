using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 行为树
/// 用于存放行为 比如 巡逻 待机 攻击 等
/// </summary>
[Serializable]
public class StatusInfo_Ob : GameNode_Ob
{
    [SerializeField] string name;
    [SerializeField][MinValue(1)] int weight = 1;
    public override int Weight => weight;
    [SerializeField] ConditionBox conditionBox = new();
    public override ConditionBox ConditionBox => conditionBox;
    [SerializeReference] List<IGameNode_Ob> childs = new();
    public override IReadOnlyList<IGameNode_Ob> Childs => childs;
    public IGameNode_Ob Deserialize(int index) => Childs[index];
}
