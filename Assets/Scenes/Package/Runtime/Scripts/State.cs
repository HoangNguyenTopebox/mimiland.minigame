using UnityEngine;

namespace mimiland.minigame
{
    public class State : BaseState
    {
        protected override GameData GameData { get; set; }
        protected override StateMachine StateMachine { get; set; }

        public override void OnEnter(StateMachine stateMachine, IGameData gameData = null)
        {
            StateMachine = stateMachine;
            GameData = (GameData)gameData;

            Debug.Log($"Entered {GetType().Name} state with {GameData.TimeTillNextState} seconds left.");
        }

        public override void OnExit(IGameData gameData = null)
        {
        }

        public override void OnUpdate()
        {
        }
    }
}