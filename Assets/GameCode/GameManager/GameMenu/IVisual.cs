using DG.Tweening;
public interface IVisual
{
    Sequence Enter();
    Sequence Exit();
    void SetActive(bool v);
}
