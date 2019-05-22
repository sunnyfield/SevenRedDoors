﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : IPlayerState
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

    public void StateUpdate(PlayerController player)
    { }

    public void OnExit(PlayerController player)
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
                return player.idleState;
        }
        
        return null;
    }
}
