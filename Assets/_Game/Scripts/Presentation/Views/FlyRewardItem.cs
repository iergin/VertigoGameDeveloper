using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class FlyRewardItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _iconImage;

        public RectTransform Rect => _rectTransform;

        public void SetIcon(Sprite icon)
        {
            _iconImage.sprite = icon;
            _iconImage.enabled = icon != null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_rectTransform == null) _rectTransform = transform as RectTransform;
            if (_iconImage == null) _iconImage = GetComponentInChildren<Image>(true);
            if (_iconImage != null) _iconImage.raycastTarget = false;
        }
#endif
    }
}
