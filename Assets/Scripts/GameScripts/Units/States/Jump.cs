using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : IPlayerState
{
    private const string name = "Jump";
    public string GetName()
    {
        return name;
    }
    public virtual void OnEnter(PlayerController player, MoveInput move, ActionInput action)
    {
        player.SetDrag(1f);
        if(action == ActionInput.JUMP) player.Jump();
        switch (move)
        {
            case MoveInput.RIGHT:
                player.MoveRight();
                break;
            case MoveInput.LEFT:
                player.MoveLeft();
                break;
            case MoveInput.NONE:
                player.Stop();
                break;
        }
        player.SetAnimation((int)AnimationState.IDLE);
    }

    public void StateUpdate(PlayerController player)
    { }

    public void OnExit(PlayerController player)
    { }

    public virtual IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        switch (move)
        {
            case MoveInput.RIGHT:
                player.MoveRight();
                break;
            case MoveInput.LEFT:
                player.MoveLeft();
                break;
            case MoveInput.NONE:
                player.Stop();
                break;
        }

        return null;
    }
}
