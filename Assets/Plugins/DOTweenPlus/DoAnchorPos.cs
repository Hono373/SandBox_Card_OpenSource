using UnityEngine;
namespace DG.Tweening
{
    [System.Serializable]
    public class DoAnchorPos : ITweenPlus
    {
        [SerializeField] private Ease _ease = Ease.Linear;
        public Tween Create(RectTransform t, Vector2 pos, float duration)
        {
            if (t == null)
            {
                Debug.LogError("RectTransform 参数不能为空！");
                return null;
            }

            Tween tween = t.DOAnchorPos(pos, duration)
                           .SetEase(_ease);

            return tween;
        }
    }
}
