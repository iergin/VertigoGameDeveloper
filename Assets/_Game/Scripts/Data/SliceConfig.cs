using System;
using UnityEngine;

namespace Vertigo.Data
{
    [Serializable]
    public sealed class SliceConfig
    {
        [Tooltip("Is this slice a bomb? If checked, the reward fields are ignored.")]
        [SerializeField] private bool _isBomb;

        [Tooltip("Reward definition (required unless this is a bomb).")]
        [SerializeField] private RewardDefinitionSO _reward;

        [Tooltip("Base amount at zone 1 (e.g. 100). Scales up as zones progress.")]
        [SerializeField, Min(1)] private int _baseAmount = 1;

        [Tooltip("Draw weight. Equal weights make a fair wheel; the bomb can be lower.")]
        [SerializeField, Min(0f)] private float _weight = 1f;

        public bool IsBomb => _isBomb;
        public RewardDefinitionSO Reward => _reward;
        public int BaseAmount => _baseAmount;
        public float Weight => _weight;
    }
}

