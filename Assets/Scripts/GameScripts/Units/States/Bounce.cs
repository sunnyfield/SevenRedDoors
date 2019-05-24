using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : IPlayerState
{
    private const float bounceTime = 0.15f;
    private float timer = 0;
    private const string name = "Bounce";

    public string GetName() { return name; }

    public virtual void OnEnter(PlayerController player, MoveInput move, ActionInput action)
    {
        player.Stop();
        player.SetDrag(1f);
        timer = bounceTime;
        player.SetAnimation((int)AnimationState.IDLE);
    }

    public void StateUpdate(PlayerController player)
    {
        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            if (player.IsGrounded()) player.SetIdleState();
            else player.SetJumpState();
        }
    }

    public virtual IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action) { return null; }
}
