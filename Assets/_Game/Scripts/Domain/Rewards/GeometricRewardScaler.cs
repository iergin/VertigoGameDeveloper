using System;

namespace Vertigo.Domain.Rewards
{
    public sealed class GeometricRewardScaler
    {
        private readonly float _growthPerZone;

        public GeometricRewardScaler(float growthPerZone)
        {
            if (growthPerZone <= 0f)
                throw new ArgumentOutOfRangeException(nameof(growthPerZone));
            _growthPerZone = growthPerZone;
        }

        public int Scale(int baseAmount, int zoneIndex)
        {
            if (zoneIndex <= 0)
                throw new ArgumentOutOfRangeException(nameof(zoneIndex));
            if (baseAmount <= 0)
                return baseAmount;

            double multiplier = Math.Pow(_growthPerZone, zoneIndex - 1);
            int scaled = (int)Math.Round(baseAmount * multiplier, MidpointRounding.AwayFromZero);
            return Math.Max(1, scaled);
        }
    }
}

