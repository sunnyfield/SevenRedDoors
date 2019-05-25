using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : BaseState
{
    private new const string name = "Run";

    private MoveInput moveState = MoveInput.NONE;

    public override void OnEnter(UnitScript unit, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE)
    {
        unit.SetDrag(1f);
        switch (move)
        {
            case MoveInput.RIGHT:
                unit.MoveRight();
                break;
            case MoveInput.LEFT:
                unit.MoveLeft();
                break;
            case MoveInput.NONE:
                break;
        }
        moveState = move;
        unit.SetAnimation((int)AnimationState.RUN);
    }

    //public override void OnEnter(PlayerController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE)
    //{
    //    Debug.Log("player's on enter");
    //}

    public override IState HandleInput(UnitScript unit, MoveInput move, ActionInput action)
    {
        if (action == ActionInput.JUMP) return unit.jumpState;
        switch (move)
        {
            case MoveInput.RIGHT:
                if (moveState != move)
                {
                    unit.MoveRight();
                    moveState = move;
                }
                break;
            case MoveInput.LEFT:
                if (moveState != move)
                {
                    unit.MoveLeft();
                    moveState = move;
                }
                break;
            case MoveInput.NONE:
                if (action == ActionInput.FIRE) return unit.attackState;
                else return unit.idleState;
        }
        return null;
    }
}
