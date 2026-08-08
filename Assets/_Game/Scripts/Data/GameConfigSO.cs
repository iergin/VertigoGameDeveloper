using UnityEngine;
using Vertigo.Domain.Zones;

namespace Vertigo.Data
{
    [CreateAssetMenu(
        fileName = "game_config",
        menuName = "Vertigo/Game Config",
        order = 2)]
    public sealed class GameConfigSO : ScriptableObject
    {
        [Header("Zone Intervals")]
        [Tooltip("How often a safe (silver, no bomb) zone occurs.")]
        [SerializeField, Min(2)] private int _safeInterval = 5;

        [Tooltip("How often a super (golden, no bomb) zone occurs.")]
        [SerializeField, Min(2)] private int _superInterval = 30;

        [Header("Reward Scaling")]
        [Tooltip("Per-zone reward growth multiplier (1.15 = ~15% increase).")]
        [SerializeField, Min(1f)] private float _rewardGrowthPerZone = 1.15f;

        [Header("Wheels")]
        [SerializeField] private WheelConfigSO _normalWheel;
        [SerializeField] private WheelConfigSO _safeWheel;
        [SerializeField] private WheelConfigSO _superWheel;

        [Header("Randomness")]
        [Tooltip("If checked, a fixed seed is used (reproducible demo).")]
        [SerializeField] private bool _useFixedSeed;
        [SerializeField] private int _seed = 12345;

        public int SafeInterval => _safeInterval;
        public int SuperInterval => _superInterval;
        public float RewardGrowthPerZone => _rewardGrowthPerZone;
        public bool UseFixedSeed => _useFixedSeed;
        public int Seed => _seed;

        public WheelConfigSO WheelFor(ZoneType zoneType)
        {
            switch (zoneType)
            {
                case ZoneType.Super: return _superWheel;
                case ZoneType.Safe: return _safeWheel;
                default: return _normalWheel;
            }
        }
    }
}

