using Vertigo.Domain.StateMachine;

namespace Vertigo.Presentation.Game.States
{
    public sealed class BombState : StateBase
    {
        private readonly GameContext _ctx;

        public BombState(GameContext ctx)
        {
            _ctx = ctx;
        }

        public override void Enter()
        {
            _ctx.RunWallet.Clear();
            _ctx.Result.ShowBomb(OnDismiss);
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

