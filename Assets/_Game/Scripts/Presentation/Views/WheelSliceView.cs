using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _amountText;

        public RectTransform RectTransform => _rectTransform;

        public void SetReward(Sprite icon, string amountLabel)
        {
            ApplyIcon(icon);
            _amountText.text = amountLabel;
            _amountText.enabled = true;
        }

        public void SetBomb(Sprite bombIcon)
        {
            ApplyIcon(bombIcon);
            _amountText.text = string.Empty;
            _amountText.enabled = false;
        }

        private void ApplyIcon(Sprite icon)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_rectTransform == null) _rectTransform = transform as RectTransform;
            if (_iconImage == null) _iconImage = GetComponentInChildren<Image>(true);
            if (_amountText == null) _amountText = GetComponentInChildren<TMP_Text>(true);
            if (_iconImage != null) _iconImage.raycastTarget = false;
            if (_amountText != null) _amountText.raycastTarget = false;
        }
#endif
    }
}

