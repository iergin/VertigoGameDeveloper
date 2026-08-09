using Vertigo.Domain.Spin;
using Vertigo.Domain.StateMachine;

namespace Vertigo.Presentation.Game.States
{
    public sealed class ResolveState : StateBase
    {
        private readonly GameContext _ctx;
        private readonly SpinResult _result;

        public ResolveState(GameContext ctx, SpinResult result)
        {
            _ctx = ctx;
            _result = result;
        }

        public override void Enter()
        {
            if (_result.IsBomb)
            {
                _ctx.Machine.ChangeState(new BombState(_ctx));
                return;
            }

            _ctx.Machine.ChangeState(new CollectState(_ctx, _result));
        }
    }
}

