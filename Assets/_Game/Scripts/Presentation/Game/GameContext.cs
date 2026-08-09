using System.Collections.Generic;
using Vertigo.Data;
using Vertigo.Domain.Rewards;
using Vertigo.Domain.Spin;
using Vertigo.Domain.StateMachine;
using Vertigo.Domain.Zones;
using Vertigo.Presentation.Views;

namespace Vertigo.Presentation.Game
{
    public sealed class GameContext
    {
        public GameConfigSO Config { get; }
        public IntervalZoneClassifier Classifier { get; }
        public SpinResolver Resolver { get; }
        public WheelSampler Sampler { get; }
        public RewardWallet RunWallet { get; }
        public StateMachine Machine { get; }

        public WheelView Wheel { get; }
        public ZoneBarView ZoneBar { get; }
        public GameButtonsView Buttons { get; }
        public ResultPopupView Result { get; }
        public WalletView WalletView { get; }
        public RewardCollectAnimator CollectAnimator { get; }
        public RewardCatalogSO Catalog { get; }

        public int CurrentZone { get; set; } = 1;

        public IReadOnlyList<SliceConfig> CurrentSlices { get; set; }

        public GameContext(
            GameConfigSO config,
            IntervalZoneClassifier classifier,
            SpinResolver resolver,
            WheelSampler sampler,
            RewardWallet runWallet,
            StateMachine machine,
            WheelView wheel,
            ZoneBarView zoneBar,
            GameButtonsView buttons,
            ResultPopupView result,
            WalletView walletView,
            RewardCollectAnimator collectAnimator,
            RewardCatalogSO catalog)
        {
            Config = config;
            Classifier = classifier;
            Resolver = resolver;
            Sampler = sampler;
            RunWallet = runWallet;
            Machine = machine;
            Wheel = wheel;
            ZoneBar = zoneBar;
            Buttons = buttons;
            Result = result;
            WalletView = walletView;
            CollectAnimator = collectAnimator;
            Catalog = catalog;
        }

        public ZoneType CurrentZoneType => Classifier.Classify(CurrentZone);

        public WheelConfigSO CurrentWheelConfig => Config.WheelFor(CurrentZoneType);

        public bool CanLeaveHere
        {
            get
            {
                ZoneType type = CurrentZoneType;
                return type == ZoneType.Safe || type == ZoneType.Super;
            }
        }

        public void ResetRun()
        {
            CurrentZone = 1;
            RunWallet.Clear();
        }
    }
}

