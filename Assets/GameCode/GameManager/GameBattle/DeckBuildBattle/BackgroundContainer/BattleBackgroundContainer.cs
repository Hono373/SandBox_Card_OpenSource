using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleBackgroundContainer : MonoBehaviour
{
    Image image;
    RectTransform RectT;
    [SerializeField] DOShakeAnchorPos doShake;
    internal void Init(DeckBuildBattle ddb)
    {
        this.GetComponentSafety(out image);
        this.GetComponentSafety(out RectT);

        LoadData(ddb);
    }
    void LoadData(DeckBuildBattle ddb)
    {
        var info = ddb.GetInfo();
        var sprite = info.backGround;
        image.sprite = sprite;
    }
    public void Shake(Sequence seq, float duration)
    {
        var shake = doShake.Create(RectT, duration);
        shake.OnComplete(() => DeckBuildBattle.GetUI().backgroundContainer.GetComponent<RectTransform>().anchoredPosition = new(0, 0));
        seq.Join(shake);
    }
}
