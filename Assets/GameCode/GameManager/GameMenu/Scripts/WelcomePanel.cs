using DG.Tweening;
using System;
using UnityEngine;
public class WelcomePanel : MonoBehaviour
{
    [SerializeField] GameMenuObj gameMenuObj;
    UIGroup uiGroup;
    [SerializeReference] UIAnim leave;
    [Range(0, 2)][SerializeField] float delay;
    public void Init(GameMenuObj obj)
    {
        gameMenuObj = obj;
        UIGroup.Create(this, out uiGroup);
    }
    public Tween Leave()
    {
        uiGroup.SetActive(true);
        var sequeue = GameAnim.AutoPlay(); ;
        sequeue.AppendInterval(delay);
        sequeue.Append(leave.Tween(uiGroup));
        sequeue.onComplete += SetWelcomRayFalse;
        return sequeue;
    }
    internal void SetActive(bool v)
    {
        uiGroup.SetActive(v);
    }
    void SetWelcomRayFalse()
    {
        uiGroup.Ray(false);
    }
    public bool IsShow => uiGroup.CG.alpha == 1;
}
