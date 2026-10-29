using Like.DrawAnim;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StoryDrawContainer : MonoBehaviour, IAnimManager
{
    StoryObj manager;
    [HideInInspector] public UIGroup uiGroup;
    public Dictionary<DrawKey, IStoryDrawBox> dic = new();
    public List<IStoryDrawBox> bin = new();
    public void Init(StoryObj manager)
    {
        UIGroup.Create(this, out uiGroup);
        this.manager = manager;
    }
    public Sprite Background
    {
        get => manager.background.Sprite;
        set => manager.background.Sprite = value;
    }
    public IStoryDrawBox DrawBox(DrawKey key)
    {
        if (key == DrawKey.None)
        {
            throw new($"DrawKey is None");
        }
        if (!dic.TryGetValue(key, out var box))
        {
            if (bin.Count != 0)
            {
                box = bin[0];
                bin.RemoveAt(0);
            }
            else
            {
                box = GameAsset.Instantiate<StoryDrawBox>(transform).Init(manager);
            }
            dic.Add(key, box);
        }
        return box;
    }
    public void BoxCleaner(DrawKey key)
    {
        if (dic.TryGetValue(key, out var box))
        {
            bin.Add(box);
            dic.Remove(key);
            box.SetActive(false);
        }
    }
    internal async Task Fresh(DialogueInfo info)
    {
        List<Task> taskList = new();
        if (info.anims.Count == 0) return;
        foreach (var anim in info.anims)
        {
            var drawAnim = (Anim)anim;
            if (drawAnim.parallel)
            {
                taskList.Add(drawAnim.Play(this));
            }
            else
            {
                await Task.WhenAll(taskList);
                taskList.Add(drawAnim.Play(this));
            }
        }
        await Task.WhenAll(taskList);
    }
}
