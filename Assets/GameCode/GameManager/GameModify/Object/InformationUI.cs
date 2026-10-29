using TMPro;
using UnityEngine;

/// <summary>修饰（Modifier）的悬浮详细说明面板：PointerEnter 时实例化，PointerExit 时销毁</summary>
public class InformationUI : MonoBehaviour
{
    CanvasGroup cg;
    RectTransform rectT;
    TextMeshProUGUI tmp;
    /// <summary>面板上下留白：面板高 = 文本高 + 2 * padding，TMP 横向宽度由预制体的 rect 自己定</summary>
    [SerializeField] float padding = 24f;
    public CanvasGroup CG => cg;
    public RectTransform RectT => rectT;
    public void Init()
    {
        this.GetComponentSafety(out cg);
        this.GetComponentSafety(out rectT);
        this.CheckInChild(out tmp);
        cg.blocksRaycasts = false;
        cg.alpha = 0;
    }
    public void Refresh(string text)
    {
        tmp.text = text;
    }
    public void Hide()
    {
        cg.alpha = 0;
    }
    /// <summary>宽度取根节点 rect 的固定值，高度按换行后的文本高度撑开</summary>
    /// <param name="anchorPosition">锚点世界坐标，取 modifier 所属单位 UI 的中心</param>
    /// <param name="anchorHalfWidth">锚点对象的半宽（世界单位）</param>
    /// <param name="right">true 挂锚点右侧、false 挂左侧；面板中心 = 锚点 ± (锚点半宽 + 面板半宽)</param>
    public void Show(Vector3 anchorPosition, float anchorHalfWidth, bool right)
    {
        tmp.ForceMeshUpdate();
        float height = tmp.preferredHeight;
        tmp.rectTransform.sizeDelta = new Vector2(tmp.rectTransform.sizeDelta.x, height);
        rectT.sizeDelta = new Vector2(rectT.sizeDelta.x, height + padding * 2f);
        float offset = anchorHalfWidth + rectT.rect.width * rectT.lossyScale.x * 0.5f;
        rectT.position = anchorPosition + new Vector3(right ? offset : -offset, 0, 0);
        cg.alpha = 1;
    }
}
