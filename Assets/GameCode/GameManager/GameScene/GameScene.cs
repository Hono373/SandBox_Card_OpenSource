using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class GameScene
{
    public static GameScene Instance { get; internal set; }

    SceneObj obj;
    GameManager gameManager;

    public static void Create(GameManager instance)
    {
        Instance = new GameScene { gameManager = instance };
    }

    internal static SceneObj GetObj()
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
        var sequence = GameAnim.AutoPlay();
        sequence.Join(GameLoading.GetObj().Enter());
        sequence.onComplete += Instance.CreateMap;
        return sequence;
    }

    void CreateMap()
    {
        GetObj().map.Create();
    }

    public static Sequence Exit() => GetObj().Exit();
    public static void SetActive(bool v) => GetObj().SetActive(v);
}
