using Vertigo.Domain.StateMachine;

namespace Vertigo.Presentation.Game.States
{
    public sealed class CashOutState : StateBase
    {
        private readonly GameContext _ctx;

        public CashOutState(GameContext ctx)
        {
            _ctx = ctx;
        }

        public override void Enter()
        {
            _ctx.Result.ShowCashOut(_ctx.RunWallet.Snapshot(), _ctx.Catalog, OnDismiss);
        }

        public override void Exit()
        {
            _ctx.Result.Hide();
        }

        private void OnDismiss()
        {
            _ctx.ResetRun();
            _ctx.Machine.ChangeState(new IdleState(_ctx));
        }
    }
}
