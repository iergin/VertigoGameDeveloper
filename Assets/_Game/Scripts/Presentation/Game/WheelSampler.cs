using System;
using System.Collections.Generic;
using Vertigo.Data;

namespace Vertigo.Presentation.Game
{
    public sealed class WheelSampler
    {
        private readonly Random _random;

        public WheelSampler(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public List<SliceConfig> Sample(IReadOnlyList<SliceConfig> pool, int slotCount)
        {
            var rewards = new List<SliceConfig>();
            SliceConfig bomb = null;
            foreach (SliceConfig slice in pool)
            {
                if (slice.IsBomb) bomb = slice;
                else rewards.Add(slice);
            }

            var result = new List<SliceConfig>(slotCount);
            int rewardSlots = bomb != null ? slotCount - 1 : slotCount;

            var remaining = new List<SliceConfig>(rewards);
            for (int i = 0; i < rewardSlots; i++)
            {
                if (remaining.Count == 0)
                    remaining.AddRange(rewards);
                if (remaining.Count == 0)
                    break;

                int index = _random.Next(remaining.Count);
                result.Add(remaining[index]);
                remaining.RemoveAt(index);
            }

            if (bomb != null)
                result.Add(bomb);

            Shuffle(result);
            return result;
        }

        private void Shuffle(List<SliceConfig> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
