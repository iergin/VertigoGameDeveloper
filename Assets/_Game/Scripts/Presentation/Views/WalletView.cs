using System.Collections.Generic;
using UnityEngine;
using Vertigo.Data;
using Vertigo.Domain.Rewards;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class WalletView : MonoBehaviour
    {
        [SerializeField] private RectTransform _content;
        [SerializeField] private WalletEntryView _entryPrefab;

        private readonly Dictionary<string, WalletEntryView> _entries = new Dictionary<string, WalletEntryView>();
        private RewardWallet _wallet;
        private RewardCatalogSO _catalog;

        public void Initialize(RewardWallet wallet, RewardCatalogSO catalog)
        {
            if (_wallet != null)
                _wallet.Changed -= OnWalletChanged;

            _wallet = wallet;
            _catalog = catalog;
            _wallet.Changed += OnWalletChanged;
            ClearEntries();
        }

        private void OnDestroy()
        {
            if (_wallet != null)
                _wallet.Changed -= OnWalletChanged;
        }

        private void OnWalletChanged()
        {
            if (_wallet.IsEmpty)
                ClearEntries();
        }

        public RectTransform EnsureEntry(string rewardId)
        {
            if (!_entries.TryGetValue(rewardId, out WalletEntryView entry))
            {
                entry = Instantiate(_entryPrefab, _content);
                entry.SetIcon(_catalog != null ? _catalog.IconFor(rewardId) : null);
                entry.SetCount(_wallet != null ? _wallet.AmountOf(rewardId) : 0);
                _entries[rewardId] = entry;
            }
            return entry.IconRect;
        }

        public void RefreshCount(string rewardId)
        {
            if (_entries.TryGetValue(rewardId, out WalletEntryView entry))
                entry.SetCount(_wallet != null ? _wallet.AmountOf(rewardId) : 0);
        }

        private void ClearEntries()
        {
            foreach (var pair in _entries)
                if (pair.Value != null)
                    Destroy(pair.Value.gameObject);
            _entries.Clear();
        }
    }
}
