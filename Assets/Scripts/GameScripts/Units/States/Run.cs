using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : IPlayerState, IZombieState
{
    private const string name = "Run";
    public string GetName()
    {
        return name;
    }

    public virtual void OnEnter(PlayerController player, MoveInput move, ActionInput action)
    {
        player.SetDrag(1f);
        switch (move)
        {
            case MoveInput.RIGHT:
                player.MoveRight();
                break;
            case MoveInput.LEFT:
                player.MoveLeft();
                break;
            case MoveInput.NONE:
                break;
        }
        player.SetAnimation((int)AnimationState.RUN);
    }

    public virtual void OnEnter(ZombieController zombie, MoveInput move, ActionInput action)
    {
        zombie.SetDrag(1f);
        switch (move)
        {
            case MoveInput.RIGHT:
                zombie.MoveRight();
                break;
            case MoveInput.LEFT:
                zombie.MoveLeft();
                break;
            case MoveInput.NONE:
                break;
        }
        zombie.SetAnimation((int)AnimationState.RUN);
    }

    public void StateUpdate(PlayerController player)
    { }

    public void StateUpdate(ZombieController zombie)
    { }

    public virtual IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        if (action == ActionInput.JUMP) return player.jumpState;
        switch(move)
        {
            case MoveInput.RIGHT:
                player.MoveRight();
                break;
            case MoveInput.LEFT:
                player.MoveLeft();
                break;
            case MoveInput.NONE:
                if (action == ActionInput.FIRE) return player.fireState;
                else return player.idleState;
        }
        
        return null;
    }

    public virtual IZombieState HandleInput(ZombieController zombie, MoveInput move, ActionInput action)
    {
        switch (move)
        {
            case MoveInput.RIGHT:
                zombie.MoveRight();
                break;
            case MoveInput.LEFT:
                zombie.MoveLeft();
                break;
            case MoveInput.NONE:
                if (action == ActionInput.FIRE) return zombie.attackState;
                else return zombie.idleState;
        }

        return null;
    }
}
