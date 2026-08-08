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

        private readonly List<WalletEntryView> _entries = new List<WalletEntryView>();
        private RewardWallet _wallet;
        private RewardCatalogSO _catalog;

        public void Initialize(RewardWallet wallet, RewardCatalogSO catalog)
        {
            if (_wallet != null)
                _wallet.Changed -= Rebuild;

            _wallet = wallet;
            _catalog = catalog;
            _wallet.Changed += Rebuild;
            Rebuild();
        }

        private void OnDestroy()
        {
            if (_wallet != null)
                _wallet.Changed -= Rebuild;
        }

        private void Rebuild()
        {
            ClearEntries();
            if (_wallet == null) return;

            foreach (RewardGrant grant in _wallet.Snapshot())
            {
                WalletEntryView entry = Instantiate(_entryPrefab, _content);
                Sprite icon = _catalog != null ? _catalog.IconFor(grant.RewardId) : null;
                entry.Set(icon, grant.Amount);
                _entries.Add(entry);
            }
        }

        private void ClearEntries()
        {
            for (int i = 0; i < _entries.Count; i++)
                if (_entries[i] != null)
                    Destroy(_entries[i].gameObject);
            _entries.Clear();
        }
    }
}

