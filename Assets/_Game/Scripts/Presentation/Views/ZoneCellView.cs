using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.Domain.Zones;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class ZoneCellView : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _numberText;

        [Header("Type Colors")]
        [SerializeField] private Color _normalColor = new Color(0.20f, 0.45f, 0.75f);
        [SerializeField] private Color _safeColor = new Color(0.25f, 0.55f, 0.20f);
        [SerializeField] private Color _superColor = new Color(0.85f, 0.65f, 0.15f);

        public void SetCell(int zoneNumber, ZoneType type, bool isCurrent)
        {
            _numberText.text = zoneNumber.ToString();

            Color color = ColorFor(type);

            _background.color = isCurrent ? color : color * 0.6f;

            transform.localScale = isCurrent ? Vector3.one * 1.12f : Vector3.one;
        }

        private Color ColorFor(ZoneType type)
        {
            switch (type)
            {
                case ZoneType.Super: return _superColor;
                case ZoneType.Safe: return _safeColor;
                default: return _normalColor;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_background == null) _background = GetComponent<Image>();
            if (_numberText == null) _numberText = GetComponentInChildren<TMP_Text>(true);
            if (_numberText != null) _numberText.raycastTarget = false;
        }
#endif
    }
}

