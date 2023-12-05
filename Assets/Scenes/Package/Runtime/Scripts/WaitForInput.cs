using UnityEngine;

namespace mimiland.minigame
{
    public class WaitForInput : State
    {
        public override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StateMachine.ChangeState(new CheckEnding());
            }
        }
    }
}