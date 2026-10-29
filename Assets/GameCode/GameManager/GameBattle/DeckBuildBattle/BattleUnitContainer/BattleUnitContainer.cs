using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class BattleUnitContainer : MonoBehaviour
{
    public Dictionary<string, BattleUnitUI> unitObjDict = new();
    public void Init(DeckBuildBattle ddb)
    {
        SetupPlayer(ddb);
        SetupEnemies(ddb);
    }
    public BattleUnitUI InstantiateObj(BattleUnit unit)
    {
        if (unitObjDict.TryGetValue(unit.data.id, out var exist))
        {
            GameDebug.Log("BattleUnitObj 已存在，错误调用！");
            return exist;
        }
        var obj = GameAsset.Instantiate<BattleUnitUI>(transform);
        obj.Init(unit);
        unitObjDict[unit.data.id] = obj;
        return obj;
    }
    public BattleUnitUI GetUnitUI(BattleUnit unit)
    {
        if (!unitObjDict.TryGetValue(unit.data.id, out var obj))
        {
            GameDebug.Log("BattleUnitObj 不存在，错误调用！");
        }
        return obj;
    }
    void SetupPlayer(DeckBuildBattle ddb)
    {
        var data = ddb.data;
        var playerData = data.GetPlayer()[0];
        var playerInfo = UnitInfo.Get(playerData.data.key);

        var playerObj = InstantiateObj(playerData);

        playerObj.intentionUI.Close();

        playerObj.hpSliderUI.SetWidth(playerInfo.battleUnitObj.hpBarWidth);

        var battleInfo = ddb.GetInfo();
        playerObj.RectT().anchoredPosition = battleInfo.mateV2[0];
        playerObj.skeleton.SetSkeletonData(playerInfo.battleUnitObj.SkeletonDataAsset);
    }
    void SetupEnemies(DeckBuildBattle ddb)
    {
        var data = ddb.data;
        var enemyData = data.GetEnemyList();
        var battleInfo = ddb.GetInfo();

        var n = 0;
        foreach (var enemy in enemyData)
        {
            var obj = InstantiateObj(enemy);
            var info = UnitInfo.Get(enemy.data.key);

            obj.intentionUI.GetComponent<RectTransform>().anchoredPosition = info.battleUnitObj.actionPos;
            obj.hpSliderUI.SetWidth(info.battleUnitObj.hpBarWidth);
            obj.skeleton.SetSkeletonData(info.battleUnitObj.SkeletonDataAsset);
            obj.RectT().anchoredPosition = battleInfo.enemyV2[n];
            n++;
        }
    }
    public Sequence RefreshIntentionObj()
    {
        var seq = GameAnim.AutoPlay(nameof(RefreshIntentionObj));
        foreach (var obj in unitObjDict.Values)
        {
            seq.Join(obj.RefreshIntentionUI());
        }
        return seq;
    }
}
