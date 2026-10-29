using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


internal class PileCheck : MonoBehaviour, IDragHandler, IScrollHandler
{
    bool isInit = false;

    UIGroup uiGroup;
    [SerializeField] UIButton closeBtn;

    public InfiniteList infinite;


    internal void Init()
    {
        if (isInit) return;
        isInit = true;

        UIGroup.Create(this, out uiGroup);
        this.CheckInChild(out infinite).Init(Create<DisplayCard>);

        closeBtn.Init(Exit);
    }

    internal void Enter()
    {
        uiGroup.SetActive(true);
    }
    public IInfiniteObject Create<T>(Transform parent) where T : MonoBehaviour, IInfiniteObject
    {
        var card = DisplayCard.Create(parent);
        return card;
    }

    public void SetActive(bool v)
    {
        uiGroup.SetActive(v);
    }

    internal void Fresh(CardPile data)
    {
        var list = data.list;
        var sendList = new List<IInfiniteData>(list);
        infinite.Fresh(sendList);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var dis = eventData.delta;
        infinite.OnDrag(dis * new Vector2(0, 1));
    }

    internal void Exit()
    {
        uiGroup.SetActive(false);
    }

    public void OnScroll(PointerEventData eventData)
    {
        infinite.OnDrag(eventData.scrollDelta * new Vector2(0, -100));
    }

}
