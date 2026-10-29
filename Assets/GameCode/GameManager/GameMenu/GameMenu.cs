using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

[Serializable]
public class GameMenu
{
    public static GameMenu Instance { get; internal set; }

    [ShowInInspector, ReadOnly] GameMenuObj obj;
    GameManager gameManager;

    internal static void Create(GameManager instance)
    {
        Instance = new GameMenu { gameManager = instance };
    }
    public static GameMenuObj GetObj()
    {
        if (Instance.obj == null)
        {
            Instance.obj = GameAsset.Instantiate<GameMenuObj>(Instance.gameManager.container);
        }
        return Instance.obj;
    }
    public static Sequence Enter()
    {
        var obj = GetObj();
        return obj.Enter();
    }
    public static Sequence Exit()
    {
        var obj = GetObj();
        return obj.Exit();
    }
    public static void SetActive(bool v)
    {
        var obj = GetObj();
        obj.SetActive(v);
    }
}
