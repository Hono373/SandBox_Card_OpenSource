using DG.Tweening;

public static class GameAnim
{
    public static Sequence AutoPlay(string desc = "")
    {
        var seq = DOTween.Sequence();
        seq.stringId = desc;
        return seq;
    }
    public static Sequence AutoPlay<T>()
    {
        var seq = DOTween.Sequence();
        seq.stringId = nameof(T);
        return seq;
    }
    public static Sequence Flow<T>()
    {
        var instance = DOTween.Sequence().Pause();
        instance.stringId = typeof(T).Name;
        return instance;
    }
    public static Sequence Flow(string desc = "")
    {
        var instance = DOTween.Sequence().Pause();
        instance.stringId = desc;
        return instance;
    }
}