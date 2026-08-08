using System.Collections.Generic;
using Vertigo.Data;
using Vertigo.Domain.Spin;
using Vertigo.Domain.StateMachine;

namespace Vertigo.Presentation.Game.States
{
    public sealed class SpinningState : StateBase
    {
        private readonly GameContext _ctx;

        public SpinningState(GameContext ctx)
        {
            _ctx = ctx;
        }

        public override void Enter()
        {
            _ctx.Wheel.SetInteractable(false);

            List<SliceModel> slices = WheelConfigSO.ToModels(_ctx.CurrentSlices);
            SpinResult result = _ctx.Resolver.Resolve(slices, _ctx.CurrentZone);

            _ctx.Wheel.SpinTo(result.SliceIndex, () =>
                _ctx.Machine.ChangeState(new ResolveState(_ctx, result)));
        }
    }
}

