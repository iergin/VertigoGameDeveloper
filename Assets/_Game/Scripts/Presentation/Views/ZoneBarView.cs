using System.Collections.Generic;
using UnityEngine;
using Vertigo.Domain.Zones;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class ZoneBarView : MonoBehaviour
    {
        [SerializeField] private List<ZoneCellView> _cells = new List<ZoneCellView>();

        private IntervalZoneClassifier _classifier;

        public void Initialize(IntervalZoneClassifier classifier)
        {
            _classifier = classifier;
        }

        public void SetZone(int currentZone, ZoneType currentType)
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                int zoneNumber = currentZone + i;
                ZoneType type = _classifier != null
                    ? _classifier.Classify(zoneNumber)
                    : (i == 0 ? currentType : ZoneType.Normal);

                _cells[i].SetCell(zoneNumber, type, isCurrent: i == 0);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_cells.Count == 0)
                _cells.AddRange(GetComponentsInChildren<ZoneCellView>(true));
        }
#endif
    }
}

