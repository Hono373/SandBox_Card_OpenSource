using DG.Tweening;
using Sirenix.OdinInspector;
public class AnimController
{
    [ShowInInspector] bool init;
    [ShowInInspector] OverrideSeq currentPlay = null;
    public void Start()
    {
        init = true;
    }
    public void Destroy()
    {
        init = false;
        currentPlay?.sequence.Kill();
    }
    public void Override(Sequence seq, bool needComplete)
    {
        seq.stringId += "Override";
        if (!init)
        {
            GameDebug.Log("AnimController not running, cannot add animation");
            return;
        }

        if (currentPlay != null)
        {
            if (currentPlay.needComplete)
            {
                currentPlay.sequence.Kill(true);
            }
            else
            {
                currentPlay.sequence.Kill();
            }
        }

        seq.Play();
        currentPlay = new OverrideSeq(seq, needComplete);
    }
}