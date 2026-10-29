using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class StoryDialogContainer : MonoBehaviour
{
    StoryObj storyManager;
    UIGroup uiGroup;

    StoryDialog dialog;
    StoryHitBox hitBox;

    Tween doText;

    [SerializeReference] UIAnim dialogEnter;
    [SerializeReference] UIAnim dialogLeave;
    public void Init(StoryObj manager)
    {
        storyManager = manager;

        this.CheckInChild(out dialog).Init(manager);
        this.CheckInChild(out hitBox).Init(manager);

        UIGroup.Create(this, out uiGroup);
    }

    List<string> nextKey = new();
    public void Fresh(DialogueInfo info)
    {
        nextKey.Clear();
        nextKey = info.outPort.nextGuids.ToList();

        //鍒颁簡鏂扮殑瀵圭櫧锛屽鏋滄病鏈夋枃鏈紝灏卞厛闅愯棌鎺?
        if (info.text.IsEmpty())
        {
            if (dialog.uiGroup.GetIsShow())
            {
                var tween = dialogLeave.Tween(dialog.uiGroup);
            }
        }

        storyManager.background.Fresh(info);
        _ = storyManager.drawContainer.Fresh(info);

        if (!info.text.IsEmpty())
        {
            if (!dialog.uiGroup.GetIsShow())
                dialogEnter.Tween(dialog.uiGroup);
        }

        doText = dialog.Fresh(info);

        if (info.text.IsEmpty())
        {
            GameStory.Instance.Fresh(nextKey);
        }
        else
        {
            hitBox.Interactable = true;
        }
    }
    public void GoNext()
    {
        doText?.Kill();
        if (dialog.IsDiaplayCompleted())
        {
            hitBox.Interactable = false;
            GameStory.Instance.Fresh(nextKey);
        }
    }
}
