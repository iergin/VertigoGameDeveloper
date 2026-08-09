using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.Data;
using Vertigo.Domain.Rewards;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class ResultPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _bodyText;
        [SerializeField] private Button _dismissButton;
        [SerializeField] private RectTransform _rewardsContainer;
        [SerializeField] private WalletEntryView _rewardEntryPrefab;

        private readonly List<WalletEntryView> _rewardEntries = new List<WalletEntryView>();
        private Action _onDismiss;

        private void Awake()
        {
            _dismissButton.onClick.AddListener(HandleDismiss);
            Hide();
        }

        private void OnDestroy()
        {
            _dismissButton.onClick.RemoveListener(HandleDismiss);
        }

        public void ShowBomb(Action onDismiss)
        {
            _onDismiss = onDismiss;
            _titleText.text = "BOOM!";
            if (_bodyText != null)
                _bodyText.text = "You hit the bomb — all your rewards are gone.\nStart over!";
            ClearRewardEntries();
            if (_root != null) _root.SetActive(true);
        }

        public void ShowCashOut(IReadOnlyList<RewardGrant> rewards, RewardCatalogSO catalog, Action onDismiss)
        {
            _onDismiss = onDismiss;
            _titleText.text = "Rewards Claimed!";
            if (_bodyText != null)
                _bodyText.text = string.Empty;
            BuildRewardEntries(rewards, catalog);
            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            _onDismiss = null;
            ClearRewardEntries();
            if (_root != null) _root.SetActive(false);
        }

        private void BuildRewardEntries(IReadOnlyList<RewardGrant> rewards, RewardCatalogSO catalog)
        {
            ClearRewardEntries();
            if (_rewardsContainer == null || _rewardEntryPrefab == null || rewards == null)
                return;

            foreach (RewardGrant grant in rewards)
            {
                WalletEntryView entry = Instantiate(_rewardEntryPrefab, _rewardsContainer);
                entry.SetIcon(catalog != null ? catalog.IconFor(grant.RewardId) : null);
                entry.SetCount(grant.Amount);
                _rewardEntries.Add(entry);
            }
        }

        private void ClearRewardEntries()
        {
            for (int i = 0; i < _rewardEntries.Count; i++)
                if (_rewardEntries[i] != null)
                    Destroy(_rewardEntries[i].gameObject);
            _rewardEntries.Clear();
        }

        private void HandleDismiss()
        {
            Action callback = _onDismiss;
            _onDismiss = null;
            callback?.Invoke();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_dismissButton == null) _dismissButton = GetComponentInChildren<Button>(true);
        }
#endif
    }
}
