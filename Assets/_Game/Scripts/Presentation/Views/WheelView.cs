using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.Data;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class WheelView : MonoBehaviour
    {
        [Header("Structure")]
        [SerializeField] private RectTransform _rotor;
        [SerializeField] private Image _baseImage;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private RectTransform _sliceContainer;
        [SerializeField] private WheelSliceView _sliceItemPrefab;
        [SerializeField] private Sprite _bombIcon;
        [SerializeField] private float _sliceRadius = 150f;

        [Header("Spin Animation")]
        [SerializeField, Min(0.1f)] private float _spinDuration = 3.5f;
        [SerializeField, Min(1)] private int _extraTurns = 5;
        [SerializeField] private Ease _spinEase = Ease.OutCubic;

        private readonly List<WheelSliceView> _items = new List<WheelSliceView>();
        private int _sliceCount;

        public void Build(IReadOnlyList<SliceConfig> slices, Sprite baseSprite, Sprite indicatorSprite)
        {
            ClearSlices();

            if (_baseImage != null && baseSprite != null)
                _baseImage.sprite = baseSprite;

            if (_indicatorImage != null && indicatorSprite != null)
                _indicatorImage.sprite = indicatorSprite;

            _sliceCount = slices.Count;
            float step = 360f / _sliceCount;

            for (int i = 0; i < _sliceCount; i++)
            {
                SliceConfig slice = slices[i];
                WheelSliceView item = Instantiate(_sliceItemPrefab, _sliceContainer);
                item.name = $"ui_wheel_slice_{i}";

                float angleRad = (i * step) * Mathf.Deg2Rad;
                item.RectTransform.anchoredPosition =
                    new Vector2(Mathf.Sin(angleRad), Mathf.Cos(angleRad)) * _sliceRadius;

                if (slice.IsBomb)
                {
                    item.SetBomb(_bombIcon);
                }
                else
                {
                    Sprite icon = slice.Reward != null ? slice.Reward.Icon : null;
                    item.SetReward(icon, $"x{slice.BaseAmount}");
                }

                _items.Add(item);
            }

            _rotor.localEulerAngles = Vector3.zero;
        }

        public void SpinTo(int sliceIndex, Action onComplete)
        {
            if (_sliceCount <= 0)
            {
                onComplete?.Invoke();
                return;
            }

            float step = 360f / _sliceCount;

            float finalZ = sliceIndex * step + _extraTurns * 360f;

            _rotor.DOKill();
            _rotor.localEulerAngles = Vector3.zero;
            _rotor.DOLocalRotate(new Vector3(0f, 0f, finalZ), _spinDuration, RotateMode.FastBeyond360)
                .SetEase(_spinEase)
                .OnComplete(() =>
                {
                    _rotor.localEulerAngles = new Vector3(0f, 0f, sliceIndex * step);
                    onComplete?.Invoke();
                });
        }

        public void SetInteractable(bool value)
        {
        }

        public Vector3 SliceWorldPosition(int index)
        {
            if (index < 0 || index >= _items.Count || _items[index] == null)
                return _rotor.position;
            return _items[index].RectTransform.position;
        }

        private void ClearSlices()
        {
            for (int i = 0; i < _items.Count; i++)
                if (_items[i] != null)
                    Destroy(_items[i].gameObject);
            _items.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_rotor == null && transform.childCount > 0)
                _rotor = transform.GetChild(0) as RectTransform;
        }
#endif
    }
}

