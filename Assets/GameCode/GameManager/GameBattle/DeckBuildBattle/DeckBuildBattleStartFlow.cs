using DG.Tweening;
using Newtonsoft.Json;
using System;

[Serializable]
public class DeckBuildBattleStartFlow : BattleFlow
{
    [JsonIgnore] DeckBuildBattle ddb;
    [JsonIgnore] public DeckBuildBattleUI obj;

    public DeckBuildBattleStartFlow Start(DeckBuildBattle ddb)
    {
        this.ddb = ddb;
        Init();
        return this;
    }

    protected override void Init()
    {
        base.Init();
        GameAnimFlow.Enqueue(flow);
    }

    protected override void FlowBuild()
    {
        flow.AppendCallback(InstantiateObj);
        flow.AppendCallback(InitObj);
        flow.AppendCallback(LoadData);
        flow.AppendCallback(EnterPhase);
    }

    void InstantiateObj()
    {
        if (ddb.obj != null)
        {
            GameDebug.Log("DeckBuildBattleObj 已存在，错误调用！");
        }
        obj = GameAsset.Instantiate<DeckBuildBattleUI>(GameManager.Instance.container);
        ddb.obj = obj;
    }

    void InitObj()
    {
        obj.Init(ddb);
    }

    void LoadData()
    {
    }

    void EnterPhase()
    {
        ddb.phase.Start();
    }
    //我要做的是，单位创建的时候 modifier的配置或者反序列化
}
