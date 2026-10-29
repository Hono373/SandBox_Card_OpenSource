using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public class SceneFuncPanel : MonoBehaviour, IDragHandler
{
    SceneObj sceneManager;
    [SerializeField] UIButton setting;
    TextMeshProUGUI settingText;
    [SerializeField] UIButton pileCheck;
    TextMeshProUGUI pileCheckText;
    internal void Init(SceneObj manager)
    {
        sceneManager = manager;
        setting.Init(Setting);
        setting.CheckInChild(out settingText);
        settingText.text = GameLanguage.GetText("Scene_Setting");
        pileCheck.Init(Check);
        pileCheck.CheckInChild(out pileCheckText);
        pileCheckText.text = GameLanguage.GetText("Scene_Check");
    }
    void Setting()
    {
        GameDebug.Log("Setting");
    }
    void Check()
    {
        GameDebug.Log("Check");
    }
    public void OnDrag(PointerEventData eventData)
    {
        GameDebug.Log("SceneFuncPanel OnDrag");
        sceneManager.map.Drag(eventData);
    }
}
