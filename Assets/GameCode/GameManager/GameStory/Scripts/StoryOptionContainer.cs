using System.Collections.Generic;
using UnityEngine;
public class StoryOptionContainer : MonoBehaviour
{
    [SerializeField] StoryOptionBtn StoryOptionBtn;
    StoryObj storyManager;
    UIGroup uiGroup;
    [SerializeField] int top;
    [SerializeField] int bottom;
    public void Init(StoryObj manager)
    {
        storyManager = manager;

        UIGroup.Create(this, out uiGroup);
        uiGroup.SetActive(false);
    }
    internal void Fresh(List<OptionInfo> options) { }

    internal void Fresh(OptionInfo option)
    {
        uiGroup.SetActive(true);
        //var offset = GetOffset(option.textInfos.Count, Top(), bottom);
        //foreach (var Top() in option.textInfos)
        //{
        //    var obj = InstantiateAsync(StoryOptionBtn, transform);
        //    obj.LoadingObj(storyManager, game);
        //    obj.BattleCardRefresh(Top(), n, Top() - n * offset);
        //    n++;
        //}
    }
    public int GetOffset(int length, int top, int bottom)
    {
        if (length != 0)
            return (top - bottom) / length;
        else
            throw new("Zero");
    }
    public void Leave()
    {
        uiGroup.SetActive(false);
    }
}
