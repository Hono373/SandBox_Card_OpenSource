using System;
using UnityEngine;
using UnityEngine.EventSystems;
public static class GameCamera
{
    static GameManager gameManager;
    static CameraObj obj;
    internal static void Create(GameManager instance)
    {
        gameManager = instance;
        GetObj().OnUpdate += RayCheck;
    }
    public static CameraObj GetObj()
    {
        if (obj == null)
        {
            gameManager.CheckInChild<CameraObj>(out obj);
            obj.Init();
        }
        return obj;
    }
    static void RayCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                GameObject hitObject = EventSystem.current.currentSelectedGameObject;
                if (hitObject != null)
                {
                    GameDebug.Log("射线碰撞到的UI元素是 " + hitObject.name);
                }
            }
            else
            {
                GameDebug.Log("射线未碰撞到任何UI元素。");
            }
        }
    }
}
