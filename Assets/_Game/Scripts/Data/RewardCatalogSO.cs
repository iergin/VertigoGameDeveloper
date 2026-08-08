using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.Data
{
    [CreateAssetMenu(
        fileName = "reward_catalog",
        menuName = "Vertigo/Reward Catalog",
        order = 3)]
    public sealed class RewardCatalogSO : ScriptableObject
    {
        [SerializeField] private List<RewardDefinitionSO> _rewards = new List<RewardDefinitionSO>();

        private Dictionary<string, RewardDefinitionSO> _lookup;

        private void EnsureLookup()
        {
            if (_lookup != null) return;
            _lookup = new Dictionary<string, RewardDefinitionSO>();
            foreach (RewardDefinitionSO reward in _rewards)
            {
                if (reward != null && !string.IsNullOrEmpty(reward.Id))
                    _lookup[reward.Id] = reward;
            }
        }

        public RewardDefinitionSO Find(string id)
        {
            EnsureLookup();
            return _lookup.TryGetValue(id, out RewardDefinitionSO reward) ? reward : null;
        }

        public Sprite IconFor(string id) => Find(id)?.Icon;

        public string DisplayNameFor(string id)
        {
            RewardDefinitionSO reward = Find(id);
            return reward != null ? reward.DisplayName : id;
        }
    }
}

