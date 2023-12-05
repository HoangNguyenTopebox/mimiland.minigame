namespace mimiland.minigame
{
    public class StateMachine
    {
        public BaseState CurrentState { get; private set; }

        public void Initialize(BaseState startingState, IGameData gameData = null)
        {
            CurrentState = startingState;
            CurrentState.OnEnter(this, gameData);
        }

        public void ChangeState(BaseState newState)
        {
            CurrentState.OnExit();
            CurrentState = newState;
            CurrentState.OnEnter(this, new GameData(5));
        }

        public void Update()
        {
            CurrentState?.OnUpdate();
        }
    }
}