using UnityEngine;

namespace Vertigo.Data
{
    [CreateAssetMenu(
        fileName = "reward_new",
        menuName = "Vertigo/Reward Definition",
        order = 0)]
    public sealed class RewardDefinitionSO : ScriptableObject
    {
        [Tooltip("Unique id used by the game logic (e.g. 'gold', 'cash').")]
        [SerializeField] private string _id;

        [Tooltip("Display name shown in the UI.")]
        [SerializeField] private string _displayName;

        [Tooltip("Icon shown on the slice and in the reward list.")]
        [SerializeField] private Sprite _icon;

        public string Id => _id;
        public string DisplayName => _displayName;
        public Sprite Icon => _icon;

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(_id))
                _id = name;
        }
#endif
    }
}

