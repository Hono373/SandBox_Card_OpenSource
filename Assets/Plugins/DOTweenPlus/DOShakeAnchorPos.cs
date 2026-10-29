using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using UnityEngine;
namespace DG.Tweening
{
    [System.Serializable]
    public class DOShakeAnchorPos : ITweenPlus
    {
        [Header("抖动强度（幅度）- 分轴控制")]
        [SerializeField] Vector2 strength = new Vector2(2f, 0.5f); // X=横向（大），Y=上下（小）
        [Header("振动次数（3-5保证均匀）")]
        [SerializeField] int vibrato = 10;
        [Header("方向随机性（0-30，越低越规律）")]
        [SerializeField] float randomness = 90f;
        [Header("是否对齐整数（均匀抖动无需）")]
        [SerializeField] bool snapping = false;
        [Header("关闭渐隐（核心：保持幅度一致）")]
        [SerializeField] bool fadeOut = true;
        [Header("固定随机模式")]
        [SerializeField] ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full;

        public Tween Create(RectTransform target, float duration)
        {
            if (duration <= 0f)
            {
                if (Debugger.logPriority > 0)
                {
                    Debug.LogWarning("DOShakePosition: duration can't be 0, returning NULL without creating a tween");
                }

                return null;
            }

            return DOTween.Shake(() => target.anchoredPosition, x => target.anchoredPosition = x, duration, strength, vibrato, randomness, fadeOut, randomnessMode)
                 .SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake).SetOptions(snapping);
        }
    }
}