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

        public RectTransform IconRect => (RectTransform)_iconImage.transform;

        public void SetIcon(Sprite icon)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
        }

        public void SetCount(int amount)
        {
            _amountText.text = $"x{amount}";
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

