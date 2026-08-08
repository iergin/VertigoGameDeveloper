using Vertigo.Domain.StateMachine;
using Vertigo.Domain.Zones;

namespace Vertigo.Presentation.Game.States
{
    public sealed class IdleState : StateBase
    {
        private readonly GameContext _ctx;

        public IdleState(GameContext ctx)
        {
            _ctx = ctx;
        }

        public override void Enter()
        {
            ZoneType type = _ctx.CurrentZoneType;
            var config = _ctx.CurrentWheelConfig;

            _ctx.Wheel.Build(config);
            _ctx.ZoneBar.SetZone(_ctx.CurrentZone, type);
            _ctx.Wheel.SetInteractable(true);

            _ctx.Buttons.SetSpinEnabled(true);
            _ctx.Buttons.SetLeaveEnabled(_ctx.CanLeaveHere);

            _ctx.Buttons.SpinClicked += OnSpinClicked;
            _ctx.Buttons.LeaveClicked += OnLeaveClicked;
        }

        public override void Exit()
        {
            _ctx.Buttons.SpinClicked -= OnSpinClicked;
            _ctx.Buttons.LeaveClicked -= OnLeaveClicked;

            _ctx.Buttons.SetSpinEnabled(false);
            _ctx.Buttons.SetLeaveEnabled(false);
        }

        private void OnSpinClicked()
        {
            _ctx.Machine.ChangeState(new SpinningState(_ctx));
        }

        private void OnLeaveClicked()
        {
            if (_ctx.CanLeaveHere)
                _ctx.Machine.ChangeState(new CashOutState(_ctx));
        }
    }
}

