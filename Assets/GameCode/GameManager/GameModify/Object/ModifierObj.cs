using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifierObj : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    [SerializeField] Modifier manager;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI value;
    RectTransform ownerRect;
    bool ownerCached;
    public string Id => manager.data.id;
    public void Init(Modifier modifier)
    {
        this.manager = modifier;
        icon.sprite = modifier.GetInfo().Img;
        value.text = modifier.data.layer.ToString();
        GameDebug.Log($"ModifierObj.Init: key={modifier.data.key} sprite={(icon.sprite == null ? "null" : icon.sprite.name)} 图标尺寸={icon.rectTransform.rect.size}");
    }
    /// <summary>
    /// 刷新图标与层数。返回 false 表示这个 buff 已归零、图标已销毁（调用方要从自己的列表里摘掉）。
    /// 2026-10-22 补：归零时原来只 RemoveRegister + Destroy，没从 owner.data.modifiers 删，
    /// 留下 layer=0 的脏记录 —— 而壁垒/腐化这类 buff 是用 HasModifier(key) 判有无的，会被误判成"还生效"。
    /// </summary>
    public bool Refresh()
    {
        icon.sprite = manager.GetInfo().Img;
        if (manager.data.layer == 1)
        {
            value.text = string.Empty;
            return true;
        }
        if (manager.data.layer != 0)
        {
            value.text = manager.data.layer.ToString();
            return true;
        }

        var owner = manager.GetOwner();
        GameModifyCenter.RemoveRegister(manager);
        owner.data.modifiers.Remove(manager);
        Destroy(gameObject);
        return false;
    }
    internal void OnAdd(Sequence seq)
    {
        var onAdd = GameAnim.AutoPlay($"Modifier {manager.GetInfo().name} OnAdd");
        seq.Append(onAdd);
    }
    internal void OnRemove(Sequence seq)
    {
        var onRemove = GameAnim.AutoPlay($"Modifier {manager.GetInfo().name} OnRemove");
        seq.Append(onRemove);
    }
    internal void OnValueChange(Sequence seq)
    {
        var onChange = GameAnim.AutoPlay($"Modifier {manager.GetInfo().name} OnValueChange");
        seq.Append(onChange);
        onChange.onComplete = () =>
        {
            icon.sprite = manager.GetInfo().Img;
            value.text = manager.data.layer.ToString();
        };
    }
    internal void Recovery()
    {
        manager = null;
        icon.sprite = null;
        value.text = "";
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        var information = DeckBuildBattle.GetUI().InformationUI;
        var info = manager.GetInfo();
        information.Refresh($"{info.modifierName}\n{info.GetDescription(manager)}");
        var rect = GetOwnerRect();
        information.Show(rect.position, rect.rect.width * rect.lossyScale.x * 0.5f, ShowOnRight(rect.position));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        DeckBuildBattle.GetUI().InformationUI.Hide();
    }
    /// <summary>owner 单位的 UI：首次悬浮时缓存（单位 UI 是在图标创建之后才注册进 container 字典的，Init 里取不到）</summary>
    RectTransform GetOwnerRect()
    {
        if (!ownerCached)
        {
            ownerRect = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(manager.GetOwner()).RectT();
            ownerCached = true;
        }
        return ownerRect;
    }
    /// <summary>锚点落在屏幕左侧 2/3 → 面板挂右侧；否则挂左侧</summary>
    bool ShowOnRight(Vector3 anchorPosition)
    {
        var canvas = this.GetComponentInParent<Canvas>();
        var screen = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, anchorPosition);
        return screen.x < Screen.width * 2f / 3f;
    }
}
