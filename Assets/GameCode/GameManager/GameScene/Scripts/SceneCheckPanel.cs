using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneCheckPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeReference] UIAnim enterAnim;
    [SerializeReference] UIAnim exitAnim;

    SceneObj sceneManager;
    public UIGroup uiGroup;
    public LevelData Focus;

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI describe;

    [SerializeField] UIButton enter;
    TextMeshProUGUI enterText;
    internal void Init(SceneObj manager)
    {
        sceneManager = manager;

        enter.CheckInChild(out enterText);
        //?
        enterText.text = GameLanguage.GetText("Scene_Enter");
        enter.Init(PressEnter);

        UIGroup.Create(this, out uiGroup);
        uiGroup.SetActive(false);
    }
    public void Checked()
    {

    }
    private void PressEnter()
    {
    }
    internal void Enter()
    {
        uiGroup.Ray(true);
        enterAnim.Tween(uiGroup);
    }
    internal void Leave()
    {
        if (uiGroup.GetIsShow())
        {
            var tween = exitAnim.Tween(uiGroup);
            tween.onComplete += SetRayFalse;
        }
        else
        {
            SetRayFalse();
        }
    }

    private void SetRayFalse()
    {
        uiGroup.Ray(false);
    }

    internal void Set(LevelData data)
    {
        Focus = data;
        title.text = GameLanguage.GetText(data.Info.Name);
        describe.text = GameLanguage.GetText(data.Info.Describe);
    }
    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
