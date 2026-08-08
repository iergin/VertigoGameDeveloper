namespace Vertigo.Domain.StateMachine
{
    public interface IState
    {
        void Enter();

        void Exit();

        void Tick(float deltaTime);
    }
}

