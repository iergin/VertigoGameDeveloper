using System;
using System.Collections.Generic;
using Vertigo.Domain.Rewards;

namespace Vertigo.Domain.Spin
{
    public sealed class SpinResolver
    {
        private readonly Random _random;
        private readonly GeometricRewardScaler _scaler;

        public SpinResolver(GeometricRewardScaler scaler, int? seed = null)
        {
            _scaler = scaler ?? throw new ArgumentNullException(nameof(scaler));
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public SpinResult Resolve(IReadOnlyList<SliceModel> slices, int zoneIndex)
        {
            if (slices == null || slices.Count == 0)
                throw new ArgumentException("Çark en az bir dilim içermeli.", nameof(slices));

            int index = PickWeightedIndex(slices);
            SliceModel slice = slices[index];

            if (slice.IsBomb)
                return new SpinResult(index, true, default);

            int amount = _scaler.Scale(slice.BaseAmount, zoneIndex);
            return new SpinResult(index, false, new RewardGrant(slice.RewardId, amount));
        }

        private int PickWeightedIndex(IReadOnlyList<SliceModel> slices)
        {
            float total = 0f;
            for (int i = 0; i < slices.Count; i++)
                total += Math.Max(0f, slices[i].Weight);

            if (total <= 0f) return 0;

            double point = _random.NextDouble() * total;
            double cumulative = 0d;
            for (int i = 0; i < slices.Count; i++)
            {
                cumulative += slices[i].Weight;
                if (point < cumulative)
                    return i;
            }
            return slices.Count - 1;
        }
    }
}

