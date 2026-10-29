using UnityEngine.Events;
using UnityEngine.UI;
public class UIButton : Button
{
    public string key;
    public UIGroup uiGroup;
    public UIButton Init(UnityAction action)
    {
        UIGroup.Create(this, out uiGroup);
        onClick.AddListener(action);
        return this;
    }
    public void Ray(bool accessUse)
    {
        uiGroup.Ray(accessUse);
    }
    public void Alpha(bool accessSee)
    {
        uiGroup.Alpha(accessSee);
    }
    public void SetActive(bool value)
    {
        uiGroup.SetActive(value);
    }
}
