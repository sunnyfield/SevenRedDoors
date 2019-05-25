using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : BaseState
{
    private new const string name = "Jump";

    private MoveInput moveState = MoveInput.NONE;

    public override void OnEnter(PlayerController player, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE)
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

    public override IState HandleInput(PlayerController player, MoveInput move, ActionInput action)
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
