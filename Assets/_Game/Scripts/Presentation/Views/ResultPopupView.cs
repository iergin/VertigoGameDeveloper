using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class ResultPopupView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _bodyText;
        [SerializeField] private Button _dismissButton;

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
            Show("BOOM!", "You hit the bomb — all your rewards are gone.\nStart over!", onDismiss);
        }

        public void ShowCashOut(string summary, Action onDismiss)
        {
            Show("Rewards Claimed!", summary, onDismiss);
        }

        public void Hide()
        {
            _onDismiss = null;
            if (_root != null) _root.SetActive(false);
        }

        private void Show(string title, string body, Action onDismiss)
        {
            _onDismiss = onDismiss;
            _titleText.text = title;
            _bodyText.text = body;
            if (_root != null) _root.SetActive(true);
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

