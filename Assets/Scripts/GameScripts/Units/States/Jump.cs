using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : IPlayerState
{
    private const string name = "Jump";
    public string GetName() { return name; }
    private MoveInput moveState = MoveInput.NONE;
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
        moveState = move;
        player.SetAnimation((int)AnimationState.IDLE);
    }

    public void StateUpdate(PlayerController player) { }

    public virtual IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        if (moveState != move)
        {
            switch (move)
            {
                case MoveInput.RIGHT:
                    player.MoveRight();
                    moveState = move;
                    break;
                case MoveInput.LEFT:
                    player.MoveLeft();
                    moveState = move;
                    break;
                case MoveInput.NONE:
                    player.Stop();
                    moveState = move;
                    break;
            }
        }
        return null;
    }
}
