using System;

namespace Vertigo.Domain.StateMachine
{
    public sealed class StateMachine
    {
        public IState Current { get; private set; }

        public event Action<IState, IState> StateChanged;

        public void ChangeState(IState next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));

            IState previous = Current;
            previous?.Exit();
            Current = next;
            StateChanged?.Invoke(previous, next);
            next.Enter();
        }

        public void Tick(float deltaTime)
        {
            Current?.Tick(deltaTime);
        }
    }
}

