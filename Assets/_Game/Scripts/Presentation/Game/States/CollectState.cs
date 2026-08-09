using UnityEngine;
using Vertigo.Domain.Spin;
using Vertigo.Domain.StateMachine;

namespace Vertigo.Presentation.Game.States
{
    public sealed class CollectState : StateBase
    {
        private readonly GameContext _ctx;
        private readonly SpinResult _result;

        public CollectState(GameContext ctx, SpinResult result)
        {
            _ctx = ctx;
            _result = result;
        }

        public override void Enter()
        {
            string rewardId = _result.Reward.RewardId;

            if (_ctx.CollectAnimator == null || _ctx.WalletView == null)
            {
                OnCollected();
                return;
            }

            RectTransform target = _ctx.WalletView.EnsureEntry(rewardId);
            Sprite icon = _ctx.Catalog != null ? _ctx.Catalog.IconFor(rewardId) : null;
            Vector3 spawn = _ctx.Wheel.SliceWorldPosition(_result.SliceIndex);

            _ctx.CollectAnimator.Play(icon, spawn, target, OnCollected);
        }

        private void OnCollected()
        {
            _ctx.RunWallet.Add(_result.Reward);
            _ctx.WalletView?.RefreshCount(_result.Reward.RewardId);
            _ctx.CurrentZone++;
            _ctx.Machine.ChangeState(new IdleState(_ctx));
        }
    }
}
