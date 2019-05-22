using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : IPlayerState
{
    public void OnEnter(PlayerController player)
    {
        player.SetDrag(1f);
    }

    public void StateUpdate(PlayerController player)
    {

    }

    public void OnExit(PlayerController player)
    {

    }

    public IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        switch(move)
        {
            case MoveInput.RIGHT:
                player.MoveRight();
                break;
            case MoveInput.LEFT:
                player.MoveLeft();
                break;
            case MoveInput.NONE:
                return player.idleState;
        }
        
        return null;
    }
}
