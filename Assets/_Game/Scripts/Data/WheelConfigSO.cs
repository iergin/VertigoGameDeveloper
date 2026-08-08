using System.Collections.Generic;
using UnityEngine;
using Vertigo.Domain.Spin;

namespace Vertigo.Data
{
    [CreateAssetMenu(
        fileName = "wheel_new",
        menuName = "Vertigo/Wheel Config",
        order = 1)]
    public sealed class WheelConfigSO : ScriptableObject
    {
        [Tooltip("Wheel base sprite (bronze/silver/golden). This asset carries the tier.")]
        [SerializeField] private Sprite _baseSprite;

        [Tooltip("Wheel slices (clockwise), in the order shown in the mockup.")]
        [SerializeField] private List<SliceConfig> _slices = new List<SliceConfig>();

        public Sprite BaseSprite => _baseSprite;
        public IReadOnlyList<SliceConfig> Slices => _slices;
        public int SliceCount => _slices.Count;

        public List<SliceModel> ToSlices() => ToModels(_slices);

        public static List<SliceModel> ToModels(IReadOnlyList<SliceConfig> slices)
        {
            var models = new List<SliceModel>(slices.Count);
            foreach (SliceConfig slice in slices)
            {
                if (slice.IsBomb)
                {
                    models.Add(SliceModel.Bomb(slice.Weight));
                }
                else
                {
                    string id = slice.Reward != null ? slice.Reward.Id : string.Empty;
                    models.Add(SliceModel.Reward(id, slice.BaseAmount, slice.Weight));
                }
            }
            return models;
        }
    }
}

