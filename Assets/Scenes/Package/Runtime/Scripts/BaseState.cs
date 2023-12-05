namespace mimiland.minigame
{
    public abstract class BaseState
    {
        protected abstract GameData GameData { get; set; }
        protected abstract StateMachine StateMachine { get; set; }
        public abstract void OnEnter(StateMachine stateMachine, IGameData gameData = null);

        public abstract void OnExit(IGameData gameData = null);

        public abstract void OnUpdate();
    }
}