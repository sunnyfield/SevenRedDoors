using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : IPlayerState
{
    private const float reloadRate = 1f;
    private float timer = 0f;
    private const string name = "Reload";
    public string GetName()
    {
        return name;
    }
    public void OnEnter(PlayerController player, MoveInput move, ActionInput action)
    {
        player.ReloadAnimationStart();
        timer = reloadRate;
    }

    public void StateUpdate(PlayerController player)
    {
        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            if (player.AddAmmo()) timer = reloadRate;
            else player.SetIdleState();
        }
    }

    public void OnExit(PlayerController player)
    { }

    public IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        if (action == ActionInput.JUMP) return player.jumpState;
        if (move != MoveInput.NONE) return player.runState;
        if (action == ActionInput.FIRE) return player.fireState;

        return null;
    }
}
