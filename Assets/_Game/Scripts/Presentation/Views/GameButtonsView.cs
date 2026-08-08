using System;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.Presentation.Views
{
    [DisallowMultipleComponent]
    public sealed class GameButtonsView : MonoBehaviour
    {
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _leaveButton;

        public event Action SpinClicked;
        public event Action LeaveClicked;

        private void Awake()
        {
            _spinButton.onClick.AddListener(RaiseSpin);
            _leaveButton.onClick.AddListener(RaiseLeave);
        }

        private void OnDestroy()
        {
            _spinButton.onClick.RemoveListener(RaiseSpin);
            _leaveButton.onClick.RemoveListener(RaiseLeave);
        }

        public void SetSpinEnabled(bool value) => _spinButton.interactable = value;
        public void SetLeaveEnabled(bool value) => _leaveButton.interactable = value;

        private void RaiseSpin() => SpinClicked?.Invoke();
        private void RaiseLeave() => LeaveClicked?.Invoke();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_spinButton == null || _leaveButton == null)
            {
                foreach (Button button in GetComponentsInChildren<Button>(true))
                {
                    string n = button.name.ToLowerInvariant();
                    if (_spinButton == null && n.Contains("spin")) _spinButton = button;
                    if (_leaveButton == null && (n.Contains("collect") || n.Contains("leave")))
                        _leaveButton = button;
                }
            }
        }
#endif
    }
}

