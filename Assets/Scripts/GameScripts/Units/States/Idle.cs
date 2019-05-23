using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZombieState
{
    IZombieState HandleInput(ZombieController actor, MoveInput move, ActionInput action);
    void OnEnter(ZombieController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE);
    void StateUpdate(ZombieController actor);
    string GetName();
}

public interface IBigZombieState
{
    IBigZombieState HandleInput(ZombieBigController actor, MoveInput move, ActionInput action);
    void OnEnter(ZombieBigController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE);
    void StateUpdate(ZombieBigController actor);
    string GetName();
}

public interface IPlayerState
{
    IPlayerState HandleInput(PlayerController actor, MoveInput move, ActionInput action);
    void OnEnter(PlayerController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE);
    void StateUpdate(PlayerController actor);
    string GetName();
}

public class Idle : IPlayerState, IZombieState
{
    private const string name = "Idle";

    public string GetName() { return name; }

    public void OnEnter(PlayerController player, MoveInput move, ActionInput action)
    {
        player.SetDrag(1000000f);
        player.Stop();
        player.SetAnimation((int)AnimationState.IDLE);
    }

    public void OnEnter(ZombieController zombie, MoveInput move, ActionInput action)
    {
        zombie.SetDrag(1000000f);
        zombie.Stop();
        zombie.SetAnimation((int)AnimationState.IDLE);
    }

    public void StateUpdate(PlayerController player) { }

    public void StateUpdate(ZombieController zombie) { }

    public IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        if (action == ActionInput.JUMP) return player.jumpState;
        if (move != MoveInput.NONE) return player.runState;
        if (action == ActionInput.FIRE) return player.fireState;
        if (action == ActionInput.RELOAD) return player.reloadState;

        return null;
    }

    public IZombieState HandleInput(ZombieController zombie, MoveInput move, ActionInput action)
    {
        if (move != MoveInput.NONE) return zombie.runState;
        if (action == ActionInput.FIRE) return zombie.attackState;

        return null;
    }
}
