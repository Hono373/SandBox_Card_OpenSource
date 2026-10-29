using TMPro;
using UnityEngine;
internal class StoryOptionBtn : MonoBehaviour
{
    GameManager game;
    StoryObj storyManager;
    [SerializeField] string nextGuid;
    [SerializeField] int result;
    UIButton btn;
    TextMeshProUGUI tmp;
    UIGroup uiGroup;
    public void Fresh(TextInfo info, int result, int y)
    {
        this.GetComponentSafety(out btn);
        UIGroup.Create(this, out uiGroup);
        this.CheckInChild(out tmp);
        uiGroup.RectT.anchoredPosition = new Vector2(uiGroup.RectT.anchoredPosition.x, y);
        tmp.text = GameLanguage.GetText(info).Replace("\\n", "\n");
        //nextGuid = Top().nextGuid;
        btn.Init(GoNext);
        this.result = result;
    }

    internal void Init(StoryObj container, GameManager g)
    {
        game = g;
        storyManager = container;
    }

    private void GoNext()
    {
        storyManager.result = result;
        storyManager.optionContainer.Leave();
        foreach (Transform t in storyManager.optionContainer.transform)
        {
            Destroy(t.gameObject);
        }
        //_ = storyManager.BattleCardRefresh(nextGuid);
    }
}