using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameStory
{
    public static GameStory Instance { get; internal set; }

    StoryObj obj;
    GameManager gameManager;

    Dictionary<string, BaseInfo> dic = new();
    List<BaseInfo> infos = new();
    Type type;
    List<OptionInfo> conditionOptions = new();
    List<OptionInfo> commonOptions = new();
    List<OptionInfo> mergedOptions = new();
    DialogueInfo commonDialogue;
    DialogueInfo conditionDialogue;

    public static void Create(GameManager instance)
    {
        Instance = new GameStory { gameManager = instance };
    }

    internal static StoryObj GetObj()
    {
        if (Instance.obj == null)
        {
            Instance.gameManager.CheckInChild(out Instance.obj);
            Instance.obj.Init();
        }
        return Instance.obj;
    }

    public static Sequence Enter()
    {
        var queue = GameSerialize.Active.storyQueue;
        if (queue.Count == 0) return GameAnim.AutoPlay();
        var key = queue.Peek();
        var info = GameAsset.GetInstanceSo<StoryInfo>(key);
        if (info == null) throw new($"Story {key} is not exist");

        Instance.LoadingLeave(info);
        return GameAnim.AutoPlay();
    }

    void LoadingLeave(StoryInfo storyInfo)
    {
        dic.Clear();
        foreach (var info in storyInfo.list)
        {
            dic.Add(info.inPort.guid, info);
        }
        GetObj().background.Fresh(storyInfo.startInfo);
        GameLoading.GetObj().Exit();
        Fresh(storyInfo.startInfo.outPort.nextGuids);
    }

    public void Fresh(List<string> nextGuids)
    {
        infos.Clear();
        foreach (var guid in nextGuids)
        {
            infos.Add(dic[guid]);
        }
        if (infos.Count == 0) Debug.LogError($"下一节点数量为0");
        type = infos[0].GetType();

        commonDialogue = null;
        conditionDialogue = null;
        commonOptions.Clear();
        conditionOptions.Clear();
        mergedOptions.Clear();

        foreach (var info in infos)
        {
            var t = info.GetType();
            if (t != type) throw new($"{t}类型与{type}不匹配");

            switch (info)
            {
                case DialogueInfo dialogue:
                    if (dialogue.conditions.Count == 0)
                    {
                        if (commonDialogue == null)
                            commonDialogue = dialogue;
                        else
                            throw new($"CommonDialogue出现了多个对话分支");
                    }
                    else
                    {
                        if (Check(dialogue))
                            if (commonDialogue == null)
                                conditionDialogue = dialogue;
                            else
                                throw new($"ConditionDialogue出现了多个条件分支");
                    }
                    break;

                case OptionInfo option:
                    if (!option.needCondition)
                        commonOptions.Add(option);
                    else
                    {
                        if (Check(option))
                            conditionOptions.Add(option);
                    }
                    break;

                case EndInfo end:
                    GameSerialize.Save();
                    GetObj().leaveAnim.Tween(GetObj().uiGroup);
                    return;
            }
        }

        if (type == typeof(DialogueInfo))
        {
            if (commonDialogue == null)
                throw new("commonDialogue == null,缺少默认对话");

            if (conditionDialogue != null)
                GetObj().dialogContainer.Fresh(conditionDialogue);
            else
                GetObj().dialogContainer.Fresh(commonDialogue);
        }
        else if (type == typeof(OptionInfo))
        {
            if (commonOptions.Count == 0)
                throw new("commonOptions.Count == 0,缺少默认选项");

            mergedOptions = commonOptions.Concat(conditionOptions).ToList();
            GetObj().optionContainer.Fresh(mergedOptions);
        }
    }

    bool Check(DialogueInfo dialogue)
    {
        foreach (var condition in dialogue.conditions)
        {
            //if (!condition.IsTrue(game.CheckerService))
            //{ return false; }
        }
        return true;
    }

    bool Check(OptionInfo dialogue)
    {
        foreach (var condition in dialogue.conditions)
        {
            //if (!condition.IsTrue(game.CheckerService))
            //{ return false; }
        }
        return true;
    }

    public static Sequence Exit() => GetObj().Exit();
    public static void SetActive(bool v) => GetObj().SetActive(v);
}
