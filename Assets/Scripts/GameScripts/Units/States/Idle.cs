using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BaseState
{
    private new const string name = "Idle";

    public override void OnEnter(UnitScript unit, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE)
    {
        unit.SetDrag(1000000f);
        unit.Stop(true);
        unit.SetAnimation((int)AnimationState.IDLE);
    }

    public override IState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        if (action == ActionInput.JUMP) return player.jumpState;
        if (move != MoveInput.NONE) return player.runState;
        if (action == ActionInput.FIRE) return player.attackState;
        if (action == ActionInput.RELOAD) return player.reloadState;
        if (action == ActionInput.ACTIVATE) if(player.trigger != null) player.trigger.TurnOn();

        return null;
    }

    public override IState HandleInput(ZombieController zombie, MoveInput move, ActionInput action)
    {
        if (move != MoveInput.NONE) return zombie.runState;
        if (action == ActionInput.FIRE) return zombie.attackState;

        return null;
    }
}
