using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : BaseState
{
    private const float reloadRate = 1f;
    private float timer = 0f;
    private new const string name = "Reload";


    public override void OnEnter(PlayerController player, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE)
    {
        player.ReloadAnimationStart();
        timer = reloadRate;
    }

    public override void StateUpdate(PlayerController player)
    {
        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            if (player.AddAmmo()) timer = reloadRate;
            else player.SetState(player.idleState);
        }
    }

    public override IState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        if (action == ActionInput.JUMP) return player.jumpState;
        if (move != MoveInput.NONE) return player.runState;
        if (action == ActionInput.FIRE) return player.attackState;

        return null;
    }
}
