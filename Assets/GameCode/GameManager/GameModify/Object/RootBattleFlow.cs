using System;
using DG.Tweening;
using Newtonsoft.Json;

[Serializable]
public class RootBattleFlow : BattleFlow
{
    protected override void Init()
    {
        base.Init();
        Enqueue();
    }
    protected void Enqueue()
    {
        if (!GameManager.Instance.skipFlow)
        {
            GameAnimFlow.Enqueue(flow);
        }
    }
}
