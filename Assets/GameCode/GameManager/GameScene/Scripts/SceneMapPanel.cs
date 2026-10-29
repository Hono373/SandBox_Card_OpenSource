using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneMapPanel : MonoBehaviour
{
    [SerializeField] float DragXMin;
    [SerializeField] SceneLevel prefab;

    SceneObj sceneManager;
    public UIGroup uiGroup;

    internal void Init(SceneObj manager)
    {
        sceneManager = manager;
        UIGroup.Create(this, out uiGroup);
    }
    [SerializeField] int paddingY;
    [SerializeField] int paddingX;
    public void Create()
    {
        if (GameSerialize.Active.sceneData != null)
            InstantiateLevel();
    }
    SceneLevel sceneLevel;
    void InstantiateLevel()
    {
        var mapData = GameSerialize.Active.sceneData.mapData;
        var x = mapData.Length();
        int y;
        var prefabHeight = prefab.uiGroup.RectT.rect.height;
        var prefabWidth = prefab.uiGroup.RectT.rect.width;

        var xPos = sceneManager.Width.Cut(x);
        for (int i = 0; i < x; i++)
        {
            y = mapData[i].Length;
            var columnHeight = prefabHeight * (y - 1) + paddingY * (y - 1);
            var yPos = columnHeight / 2;

            for (int j = 0; j < mapData[i].Length; j++)
            {
                var level = Instantiate(prefab, transform);
                if (sceneLevel == null) sceneLevel = level;
                level.Init(sceneManager, mapData[i][j], new Vector2(xPos, yPos));
                yPos -= prefabHeight + paddingY;
            }
            uiGroup.RectT.sizeDelta = new Vector2(xPos, uiGroup.RectT.sizeDelta.y);
            xPos += prefabWidth + paddingX;
        }
        DragXMax = sceneManager.Width / 2 - sceneManager.Width.Cut(x) + prefabWidth / 2;
        DragXMin = -uiGroup.RectT.rect.width / 2;
        Focus(sceneLevel.GetScreenPos());
    }

    public void Focus(Vector2 target)
    {
        var t = DragClamp(target);
        uiGroup.RectT.DOAnchorPos(t, 0.25f).SetEase(Ease.Linear);
    }

    Vector2 pos;
    Vector2 delta;

    internal void Drag(PointerEventData eventData)
    {
        if (sceneManager.check.uiGroup.GetIsShow()) return;
        pos = uiGroup.RectT.anchoredPosition;
        delta = eventData.delta;
        uiGroup.RectT.anchoredPosition = DragClamp(new(pos.x + delta.x, pos.y)); ;
    }
    private float DragXMax;
    public Vector2 DragClamp(Vector2 v)
    {
        var x = Math.Clamp(v.x, DragXMin, DragXMax);
        return new Vector2(x, v.y);
    }
}
