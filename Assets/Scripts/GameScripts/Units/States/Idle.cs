using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action);
    void OnEnter(PlayerController player);
    void StateUpdate(PlayerController player);
    void OnExit(PlayerController player);
}

public class Idle : IPlayerState
{
    public void OnEnter(PlayerController player)
    {
        player.SetDrag(1000000f);
        player.Stop();
    }

    public void StateUpdate(PlayerController player)
    {

    }

    public void OnExit(PlayerController player)
    {

    }

    public IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        if (move != MoveInput.NONE) return player.runState;

        return null;
    }
}
