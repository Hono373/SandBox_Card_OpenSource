using DG.Tweening;
using TMPro;
using UnityEngine;

public class CostContainer : MonoBehaviour
{
    public UIGroup uiGroup;
    public TextMeshProUGUI cost;
    internal void Init(DeckBuildBattle ddb)
    {
        UIGroup.Create(this, out uiGroup);
        uiGroup.SetActive(false);
    }
    public Sequence RefreshUI(int currentCost, int maxCost)
    {
        var seq = GameAnim.AutoPlay(string.Join(nameof(CostContainer), ".", nameof(RefreshUI)));
        seq.onComplete += () => uiGroup.Alpha(true);
        seq.onComplete += () => cost.text = $"{currentCost}/{maxCost}";
        return seq;
    }
}
