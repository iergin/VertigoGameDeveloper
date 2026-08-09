using DG.Tweening;
using UnityEngine;

namespace Vertigo.Data
{
    [CreateAssetMenu(
        fileName = "reward_animation",
        menuName = "Vertigo/Reward Animation",
        order = 4)]
    public sealed class RewardAnimationSO : ScriptableObject
    {
        [Header("Spawn")]
        [Tooltip("How many flying items spawn per reward.")]
        [SerializeField, Min(1)] private int _count = 5;

        [Tooltip("Pixel size of each flying item.")]
        [SerializeField, Min(1f)] private float _itemSize = 80f;

        [Tooltip("Random scale range applied to each flying item.")]
        [SerializeField, Min(0.05f)] private float _scaleMin = 0.5f;
        [SerializeField, Min(0.05f)] private float _scaleMax = 1.2f;

        [Header("Scatter")]
        [Tooltip("Items scatter to random points within this radius from the spawn point.")]
        [SerializeField, Min(0f)] private float _scatterRadius = 120f;

        [SerializeField, Min(0.01f)] private float _scatterDuration = 0.35f;
        [SerializeField] private Ease _scatterEase = Ease.OutBack;

        [Tooltip("Pause after scattering, before flying to the wallet.")]
        [SerializeField, Min(0f)] private float _holdDelay = 0.1f;

        [Header("Fly")]
        [SerializeField, Min(0.01f)] private float _flyDuration = 0.5f;
        [SerializeField] private Ease _flyEase = Ease.InBack;

        [Tooltip("Extra delay between each item's fly start (0 = all together).")]
        [SerializeField, Min(0f)] private float _flyStagger = 0f;

        public int Count => _count;
        public float ItemSize => _itemSize;
        public float ScaleMin => _scaleMin;
        public float ScaleMax => _scaleMax;
        public float ScatterRadius => _scatterRadius;
        public float ScatterDuration => _scatterDuration;
        public Ease ScatterEase => _scatterEase;
        public float HoldDelay => _holdDelay;
        public float FlyDuration => _flyDuration;
        public Ease FlyEase => _flyEase;
        public float FlyStagger => _flyStagger;
    }
}
