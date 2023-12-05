using UnityEngine;

namespace mimiland.minigame
{
    public class ConversationState : State
    {
        public override void OnUpdate()
        {
            GameData.TimeTillNextState -= Time.deltaTime;

            if (GameData.TimeTillNextState <= 0)
            {
                StateMachine.ChangeState(new WaitForInput());
            }
        }
    }
}