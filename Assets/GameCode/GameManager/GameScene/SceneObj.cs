using DG.Tweening;
using UnityEngine;

public class SceneObj : MonoBehaviour
{
    public UIGroup uiGroup;

    [HideInInspector] public SceneBackground background;
    [HideInInspector] public SceneMapPanel map;
    [HideInInspector] public SceneCheckPanel check;
    [HideInInspector] public SceneFuncPanel funcPanel;

    public Transform temporary;

    public void Init()
    {
        this.CheckInChild(out background).Init(this);
        this.CheckInChild(out map).Init(this);
        this.CheckInChild(out check).Init(this);
        this.CheckInChild(out funcPanel).Init(this);
        UIGroup.Create(this, out uiGroup);
    }

    public Sequence Exit()
    {
        uiGroup.SetActive(false);
        return GameAnim.AutoPlay();
    }

    public void SetActive(bool v)
    {
        uiGroup.SetActive(v);
    }

    public float Width => uiGroup.RectT.rect.width;
}
