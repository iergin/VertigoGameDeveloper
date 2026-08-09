using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class WalletEntryView : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _amountText;

        private float _shownValue;
        private Tween _countTween;

        public RectTransform IconRect => (RectTransform)_iconImage.transform;

        public void SetIcon(Sprite icon)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
        }

        public void SetCount(int amount)
        {
            KillCountTween();
            _shownValue = amount;
            _amountText.text = $"x{amount}";
        }

        public void SetCountAnimated(int target, float duration)
        {
            KillCountTween();

            if (duration <= 0f || Mathf.RoundToInt(_shownValue) == target)
            {
                _shownValue = target;
                _amountText.text = $"x{target}";
                return;
            }

            _countTween = DOTween.To(() => _shownValue, value =>
                {
                    _shownValue = value;
                    _amountText.text = $"x{Mathf.RoundToInt(value)}";
                }, target, duration)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    _shownValue = target;
                    _amountText.text = $"x{target}";
                });
        }

        private void KillCountTween()
        {
            if (_countTween != null && _countTween.IsActive())
                _countTween.Kill();
            _countTween = null;
        }

        private void OnDestroy()
        {
            KillCountTween();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_iconImage == null) _iconImage = GetComponentInChildren<Image>(true);
            if (_amountText == null) _amountText = GetComponentInChildren<TMP_Text>(true);
            if (_iconImage != null) _iconImage.raycastTarget = false;
            if (_amountText != null) _amountText.raycastTarget = false;
        }
#endif
    }
}

