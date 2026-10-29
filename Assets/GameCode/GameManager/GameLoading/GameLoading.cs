using System;
using Sirenix.OdinInspector;

[Serializable]
public class GameLoading
{
    public static GameLoading Instance { get; internal set; }

    [ShowInInspector, ReadOnly] LoadingObj obj;
    GameManager gameManager;

    public static void Create(GameManager instance)
    {
        Instance = new GameLoading { gameManager = instance };
    }
    public static LoadingObj GetObj()
    {
        if (Instance.obj == null)
        {
            Instance.gameManager.CheckInChild(out Instance.obj);
            Instance.obj.Init();
        }
        return Instance.obj;
    }
}
