﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action);
    void OnEnter(PlayerController player, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE);
    void StateUpdate(PlayerController player);
    void OnExit(PlayerController player);
    string GetName();
}

public class Idle : IPlayerState
{
    private const string name = "Idle";
    public string GetName()
    {
        return name;
    }
    public void OnEnter(PlayerController player, MoveInput move, ActionInput action)
    {
        player.SetDrag(1000000f);
        player.Stop();
        player.SetAnimation((int)AnimationState.IDLE);
    }

    public void StateUpdate(PlayerController player)
    { }

    public void OnExit(PlayerController player)
    { }

    public IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        if (action == ActionInput.JUMP) return player.jumpState;
        if (move != MoveInput.NONE) return player.runState;
        if (action == ActionInput.FIRE) return player.fireState;
        if (action == ActionInput.RELOAD) return player.reloadState;

        return null;
    }
}
