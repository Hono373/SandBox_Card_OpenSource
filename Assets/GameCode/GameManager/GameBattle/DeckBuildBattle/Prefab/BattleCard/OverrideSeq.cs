using DG.Tweening;

public class OverrideSeq
{
    public readonly Sequence sequence;
    public readonly bool needComplete;
    public OverrideSeq(Sequence sequence, bool needComplete)
    {
        this.sequence = sequence;
        this.needComplete = needComplete;
    }
}
