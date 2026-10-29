using UnityEngine;

public interface IInfiniteObject
{
    Rect Rect { get; }
    void Fresh(IInfiniteData cardData);
    void SetActive(bool v);
    void SetPos(Vector2 pos);
}
