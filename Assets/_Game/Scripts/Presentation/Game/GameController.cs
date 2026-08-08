using UnityEngine;
using Vertigo.Data;
using Vertigo.Domain.Rewards;
using Vertigo.Domain.Spin;
using Vertigo.Domain.StateMachine;
using Vertigo.Domain.Zones;
using Vertigo.Presentation.Game.States;
using Vertigo.Presentation.Views;

namespace Vertigo.Presentation.Game
{
    [DisallowMultipleComponent]
    public sealed class GameController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private GameConfigSO _config;
        [SerializeField] private RewardCatalogSO _catalog;

        [Header("Views")]
        [SerializeField] private WheelView _wheelView;
        [SerializeField] private ZoneBarView _zoneBarView;
        [SerializeField] private GameButtonsView _buttonsView;
        [SerializeField] private ResultPopupView _resultView;
        [SerializeField] private WalletView _walletView;

        private StateMachine _machine;

        private void Start()
        {
            var classifier = new IntervalZoneClassifier(_config.SafeInterval, _config.SuperInterval);
            var scaler = new GeometricRewardScaler(_config.RewardGrowthPerZone);
            int? seed = _config.UseFixedSeed ? _config.Seed : (int?)null;
            var resolver = new SpinResolver(scaler, seed);

            var wallet = new RewardWallet();
            _machine = new StateMachine();

            _zoneBarView.Initialize(classifier);
            _walletView.Initialize(wallet, _catalog);

            var context = new GameContext(
                _config, classifier, resolver, wallet, _machine,
                _wheelView, _zoneBarView, _buttonsView, _resultView);

            _machine.ChangeState(new IdleState(context));
        }

        private void Update()
        {
            _machine?.Tick(Time.deltaTime);
        }
    }
}

