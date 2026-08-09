using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Vertigo.Data;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class RewardCollectAnimator : MonoBehaviour
    {
        [SerializeField] private RewardAnimationSO _settings;
        [SerializeField] private FlyRewardItem _flyItemPrefab;
        [SerializeField] private RectTransform _flyLayer;
        [SerializeField] private Canvas _canvas;

        public void Play(Sprite icon, Vector3 spawnWorldPos, RectTransform target, Action onComplete)
        {
            if (_settings == null || _flyItemPrefab == null || _flyLayer == null || icon == null || target == null)
            {
                onComplete?.Invoke();
                return;
            }

            Vector2 spawnLocal = WorldToLocal(spawnWorldPos);
            Vector2 targetLocal = WorldToLocal(target.position);

            var items = new List<RectTransform>(_settings.Count);
            for (int i = 0; i < _settings.Count; i++)
                items.Add(CreateItem(icon, spawnLocal));

            var sequence = DOTween.Sequence();

            for (int i = 0; i < items.Count; i++)
            {
                RectTransform rt = items[i];
                Vector2 scatter = spawnLocal + UnityEngine.Random.insideUnitCircle * _settings.ScatterRadius;
                sequence.Insert(0f, DOTween.To(
                    () => rt.anchoredPosition, v => rt.anchoredPosition = v, scatter, _settings.ScatterDuration)
                    .SetEase(_settings.ScatterEase));
            }

            float flyStart = _settings.ScatterDuration + _settings.HoldDelay;
            for (int i = 0; i < items.Count; i++)
            {
                RectTransform rt = items[i];
                sequence.Insert(flyStart + i * _settings.FlyStagger, DOTween.To(
                    () => rt.anchoredPosition, v => rt.anchoredPosition = v, targetLocal, _settings.FlyDuration)
                    .SetEase(_settings.FlyEase));
            }

            sequence.OnComplete(() =>
            {
                for (int i = 0; i < items.Count; i++)
                    if (items[i] != null)
                        Destroy(items[i].gameObject);
                onComplete?.Invoke();
            });
        }

        private RectTransform CreateItem(Sprite icon, Vector2 localPos)
        {
            FlyRewardItem item = Instantiate(_flyItemPrefab, _flyLayer);
            item.SetIcon(icon);

            RectTransform rt = item.Rect;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(_settings.ItemSize, _settings.ItemSize);
            rt.anchoredPosition = localPos;
            rt.localScale = Vector3.one * UnityEngine.Random.Range(_settings.ScaleMin, _settings.ScaleMax);
            return rt;
        }

        private Vector2 WorldToLocal(Vector3 worldPos)
        {
            Camera cam = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;

            Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_flyLayer, screen, cam, out Vector2 local);
            return local;
        }
    }
}
