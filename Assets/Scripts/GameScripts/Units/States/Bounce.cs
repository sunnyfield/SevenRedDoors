using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : BaseState
{
    private const float bounceTime = 0.15f;
    private float timer = 0;
    private new const string name = "Bounce";

    public override void OnEnter(PlayerController player, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE)
    {
        player.Stop(true);
        player.SetDrag(1f);
        timer = bounceTime;
        player.SetAnimation((int)AnimationState.IDLE);
    }

    public override void StateUpdate(PlayerController player)
    {
        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            if (player.IsGrounded()) player.SetState(player.idleState);
            else player.SetState(player.jumpState);
        }
    }

    public override IState HandleInput(PlayerController player, MoveInput move, ActionInput action) { return null; }
}
