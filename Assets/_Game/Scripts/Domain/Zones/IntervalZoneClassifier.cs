using System;

namespace Vertigo.Domain.Zones
{
    public sealed class IntervalZoneClassifier
    {
        private readonly int _safeInterval;
        private readonly int _superInterval;

        public IntervalZoneClassifier(int safeInterval, int superInterval)
        {
            if (safeInterval <= 0)
                throw new ArgumentOutOfRangeException(nameof(safeInterval));
            if (superInterval <= 0)
                throw new ArgumentOutOfRangeException(nameof(superInterval));

            _safeInterval = safeInterval;
            _superInterval = superInterval;
        }

        public ZoneType Classify(int zoneIndex)
        {
            if (zoneIndex <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(zoneIndex), "Zone indeksi 1 tabanlıdır (>= 1).");

            if (zoneIndex % _superInterval == 0) return ZoneType.Super;
            if (zoneIndex % _safeInterval == 0) return ZoneType.Safe;
            return ZoneType.Normal;
        }
    }
}

