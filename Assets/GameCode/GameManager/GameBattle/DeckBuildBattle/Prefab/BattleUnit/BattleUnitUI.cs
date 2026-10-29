using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BattleUnitUI : MonoBehaviour
{
    public string id;
    public string key;
    [HideInPlayMode][SerializeField] BattleCreator battleCreator;
    public BattleCreator.Team team;
    [HideInPlayMode][Button("SaveBattlePos()")] public void SaveBattlePos() => battleCreator.SaveBattlePos(this, team);
    [HideInEditorMode]
    [Button("SaveObjInfo()")]
    public void SaveObjInfo() 
    {
        if (GameEditor.IsPlaying())
        {
            var ddbInfo = DeckBuildBattle.Active.GetInfo();
            var info = UnitInfo.Get(key);
            info.battleUnitObj.hpBarWidth = (int)hpSliderUI.RectT.sizeDelta.x;
            info.battleUnitObj.actionPos = intentionUI.OriginPosition;

            if (manager.camp.camp == CampEnum.Mate)
            {
                ddbInfo.mateV2[0] = GetComponent<RectTransform>().anchoredPosition;
            }
            else
            {
                var index = DeckBuildBattle.Active.data.GetEnemyList().IndexOf(manager);
                ddbInfo.enemyV2[index] = GetComponent<RectTransform>().anchoredPosition;
            }

            GameEditor.SaveAssets(info);
        }
    }
    public static BattleUnitObjAnime anime;
    [HideInInspector] public UnitSkeleton skeleton;
    [HideInInspector] public BattleUnitIntentionBarUI intentionUI;
    [HideInInspector] public BattleUnitHPSliderUI hpSliderUI;
    [HideInInspector] public BattleUnitShieldUI shieldUI;
    [HideInInspector] public BattleUnitSelectedUI selectedUI;
    [HideInInspector] public BattleUnitModifierBarUI modifierBarUI;
    public RectTransform RectT() => rectT ??= this.GetRectTransform();
    RectTransform rectT;
    public BattleUnit manager;
    public void Init(BattleUnit manager)
    {
        this.manager = manager;
        id = manager.data.id;
        key = manager.data.key;

        this.CheckInChild(out skeleton).Init();
        this.CheckInChild(out intentionUI).Init();
        this.CheckInChild(out hpSliderUI).Init(this);
        this.CheckInChild(out shieldUI).Init(this);
        this.CheckInChild(out selectedUI).Init(this);
        this.CheckInChild(out modifierBarUI).Init(this);

        LoadData();
    }
    void LoadData()
    {
        hpSliderUI.Refresh();
        // 2026-10-20 idle 播放移到 UnitSkeleton.SetSkeletonData，这里播的还是 prefab 默认骨架的动画，
        // 紧接着换骨架 Initialize 就被重建成 setup pose，属于无效播放，故删除。
        modifierBarUI.Refresh();

        if (manager.camp.camp == CampEnum.Mate)
            intentionUI.Close();
        // 2026-10-21 敌方意图此时还没算（BattleUnit.Create 不再初始化 unitSkill），不在这里刷，
        // 由 BattlePhaseStart 的 RefreshUnitIntentionGroupFlow 写入 unitSkill 后再刷 UI。
    }
    public void FaceTo(bool toRight)
    {
        skeleton.Face(toRight);
    }
    public Sequence RefreshIntentionUI() => intentionUI.Refresh(manager);
}

