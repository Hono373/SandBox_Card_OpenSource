using DG.Tweening;
using System.Collections.Generic;

public static class GameAnimFlow
{
    static int log;
    static Queue<Sequence> queue = new();
    static Sequence current;
    public static void Enqueue(Sequence seq)
    {
        seq.intId = log++;
        seq.Pause();
        GameDebug.Log($"[{seq.intId}] {seq.stringId} 入队");

        seq.onComplete += () =>
        {
            GameDebug.Log($"[{seq.intId}] {seq.stringId} 播放完毕");
            Clear();
        };
        queue.Enqueue(seq);
        TryStart();
    }
    public static void TryStart()
    {
        if (current == null)
        {
            if (queue.Count > 0)
            {
                var seq = queue.Dequeue();
                current = seq;

                GameDebug.Log($"[{seq.intId}] {seq.stringId} 播放开始");

                current.Play();
            }
        }
    }
    public static void Clear()
    {
        var destroy = current;
        DOVirtual.DelayedCall(0.1f, () => destroy.Kill());
        current = null;
        TryStart();
    }
}
