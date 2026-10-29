using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattleArrowLine : MonoBehaviour
{
    public Image Arrow;
    public List<Image> LineImage;
    public Vector2 OriginalPosition;
    public float Curvature;

    UIGroup uiGroup;
    public void Init()
    {
        UIGroup.Create(this, out uiGroup);
        uiGroup.SetActive(false);
    }
    public ArrowStateEnum state;
    public ArrowStateEnum State
    {
        get => state;
        set
        {
            if (state == value) return;
            switch (value)
            {
                case ArrowStateEnum.Targeted:
                    foreach (var line in LineImage)
                        line.color = Color.red;
                    Arrow.color = Color.red;
                    break;
                case ArrowStateEnum.NoTarget:
                    foreach (var line in LineImage)
                        line.color = Color.white;
                    Arrow.color = Color.white;
                    break;
            }
            state = value;
        }
    }
    public bool isShow() => uiGroup.CG.alpha == 1;
    internal void Alpha(bool v)
    {
        uiGroup.Alpha(v);
    }
    public void Drag(Vector2 cardPos, Vector2 mousePos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform,
            Input.mousePosition, GameCamera.GetObj().camera, out Vector2 position);

        Vector3 start = cardPos;
        Vector3 end = new(mousePos.x, mousePos.y, 0);

        Vector3 direction = (start - end).normalized;
        Vector3 perpendicular = new(-direction.y, direction.x, direction.z);

        Vector3 midPoint = (start + end) / 2 - perpendicular * end.x / 3;

        Vector3 back = new();

        for (int i = 0; i < 16; i++)
        {
            float t = 1 - Mathf.Pow((16 - i) / 16.0f, 2);
            Vector3 current = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * midPoint + t * t * end;
            Vector3 tangent = (current - back).normalized;

            if (i == 0)
            {
                LineImage[i].rectTransform.anchoredPosition = current;
                LineImage[i].rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, (midPoint - start).normalized);
            }
            else if (i > 0 && i < 15)
            {
                LineImage[i].rectTransform.localScale = new(1 - i * (0.5f / 15), 1 - i * (0.5f / 15));
                LineImage[i].rectTransform.anchoredPosition = current;
                LineImage[i].rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, tangent);
            }
            else
            {
                Arrow.rectTransform.anchoredPosition = current;
                Arrow.rectTransform.localRotation = Quaternion.LookRotation(Vector3.forward, tangent);
            }
            back = current;
        }
    }
}
