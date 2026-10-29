using DG.Tweening;
using TMPro;
using UnityEngine;
public class StoryDialog : MonoBehaviour
{
    public UIGroup uiGroup;
    [SerializeField] TextMeshProUGUI dialog;
    [SerializeField] TextMeshProUGUI speaker;
    [SerializeField] string fullText = string.Empty;
    internal void Init(StoryObj manager)
    {
        UIGroup.Create(this, out uiGroup);

        uiGroup.SetActive(false);

        speaker.text = string.Empty;
        dialog.text = string.Empty;
    }
    internal Tween Fresh(DialogueInfo info)
    {
        speaker.text = GameLanguage.GetText(info.speaker);
        dialog.text = string.Empty;
        fullText = GameLanguage.GetText(info.text).Replace("\\n", "\n");
        return dialog.DOText(fullText, (float)fullText.Length / 10).SetEase(Ease.Linear);
    }
    internal bool IsDiaplayCompleted()
    {
        if (dialog.text != fullText)
        {
            dialog.text = fullText;
            return false;
        }
        return true;
    }
}
