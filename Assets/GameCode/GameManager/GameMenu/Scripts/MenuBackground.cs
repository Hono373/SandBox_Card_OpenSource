using UnityEngine;
using UnityEngine.UI;

public class MenuBackground : MonoBehaviour
{
    GameMenuObj gameMenuObj;
    Image image;
    UIGroup uiGroup;
    public void Init(GameMenuObj obj)
    {
        this.gameMenuObj = obj;
        this.GetComponentSafety(out image);
        UIGroup.Create(this, out uiGroup);
    }
    internal void Enter()
    {
        uiGroup.SetActive(true);
    }
    internal void Leave()
    {
        uiGroup.SetActive(false);
    }
}
