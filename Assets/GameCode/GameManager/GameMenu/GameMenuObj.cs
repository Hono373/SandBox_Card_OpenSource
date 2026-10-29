using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameMenuObj : MonoBehaviour, IVisual
{
    UIGroup uiGroup;
    WelcomePanel welcome;
    MenuPanel menu;
    MenuBackground background;
    public void Init()
    {
        UIGroup.Create(this, out uiGroup);
        this.CheckInChild(out welcome).Init(this);
        this.CheckInChild(out menu).Init(this);
        this.CheckInChild(out background).Init(this);
    }
    public Sequence Enter()
    {
        background.Enter();
        welcome.Leave();
        menu.Enter();
        return GameAnim.AutoPlay(); ;
    }
    public Sequence Exit()
    {
        uiGroup.SetActive(false);
        Destroy(gameObject, 1f);
        return GameAnim.AutoPlay(); ;
    }
    public void SetActive(bool v)
    {
        uiGroup.SetActive(v);
    }
}
