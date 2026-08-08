using System;
using System.Collections.Generic;

namespace Vertigo.Domain.Rewards
{
    public sealed class RewardWallet
    {
        private readonly Dictionary<string, int> _amountsById = new Dictionary<string, int>();

        public event Action Changed;

        public IReadOnlyDictionary<string, int> Amounts => _amountsById;

        public bool IsEmpty => _amountsById.Count == 0;

        public void Add(RewardGrant grant)
        {
            if (string.IsNullOrEmpty(grant.RewardId))
                throw new ArgumentException("Ödül id'si boş olamaz.", nameof(grant));
            if (grant.Amount <= 0)
                throw new ArgumentException("Ödül miktarı pozitif olmalı.", nameof(grant));

            _amountsById.TryGetValue(grant.RewardId, out int current);
            _amountsById[grant.RewardId] = current + grant.Amount;

            Changed?.Invoke();
        }

        public int AmountOf(string rewardId)
        {
            _amountsById.TryGetValue(rewardId, out int amount);
            return amount;
        }

        public void Clear()
        {
            if (_amountsById.Count == 0)
                return;

            _amountsById.Clear();
            Changed?.Invoke();
        }

        public IReadOnlyList<RewardGrant> Snapshot()
        {
            var list = new List<RewardGrant>(_amountsById.Count);
            foreach (var pair in _amountsById)
                list.Add(new RewardGrant(pair.Key, pair.Value));
            return list;
        }
    }
}

