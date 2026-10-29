using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnitHPSliderUI : MonoBehaviour
{
    BattleUnitUI battleUnitObj;
    Slider slider;
    TextMeshProUGUI sliderValue;
    RectTransform rectT;
    public RectTransform RectT => rectT ??= GetComponent<RectTransform>();
    public void SetWidth(float width) => RectT.sizeDelta = new(width, RectT.sizeDelta.y);
    public void Init(BattleUnitUI unitObj)
    {
        this.battleUnitObj = unitObj;

        this.GetComponentSafety(out slider);
        this.CheckInChild(out sliderValue);

        slider.interactable = false;
        slider.minValue = 0;
    }
    public void Refresh()
    {
        var data = battleUnitObj.manager.data;
        RefreshHpBar(data.hp, data.maxHP, 0.5f , out var refreshHpBar);
    }
    public void RefreshHpBar(int hp, int maxHP, float spd, out Sequence result)
    {
        result = GameAnim.AutoPlay("生命值变动");
        result.PrependCallback(SetMaxValue);
        var tp = DOTween.To(
              () => slider.value,
              value => RefreshTmp(value),
              hp,
              spd
          );
        void RefreshTmp(float value)
        {
            slider.value = value;
            sliderValue.text = $"{slider.value}/{slider.maxValue}";
        }
        void SetMaxValue()
        {
            slider.maxValue = maxHP;
        }
        result.Append(tp);
    }
}
