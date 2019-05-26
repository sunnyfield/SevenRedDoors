using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    IState HandleInput(UnitScript actor, MoveInput move, ActionInput action);
    IState HandleInput(PlayerController actor, MoveInput move, ActionInput action);
    IState HandleInput(ZombieController actor, MoveInput move, ActionInput action);
    IState HandleInput(ZombieBigController actor, MoveInput move, ActionInput action);
    IState HandleInput(FlyController actor, MoveInput move, ActionInput action);
    void OnEnter(UnitScript actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE);
    void OnEnter(PlayerController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE);
    void OnEnter(ZombieController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE);
    void OnEnter(ZombieBigController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE);
    void OnEnter(FlyController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE);
    void StateUpdate(UnitScript actor);
    void StateUpdate(PlayerController actor);
    void StateUpdate(ZombieController actor);
    void StateUpdate(ZombieBigController actor);
    void StateUpdate(FlyController actor);
    string GetName();
}

public class BaseState : IState
{
    protected const string name = "base";

    public virtual IState HandleInput(UnitScript actor, MoveInput move, ActionInput action) { Debug.Log(actor.GetType() + " base HandleInput called"); return null; }
    public virtual IState HandleInput(PlayerController actor, MoveInput move, ActionInput action) { return HandleInput(actor as UnitScript, move, action); }
    public virtual IState HandleInput(ZombieController actor, MoveInput move, ActionInput action) { return HandleInput(actor as UnitScript, move, action); }
    public virtual IState HandleInput(ZombieBigController actor, MoveInput move, ActionInput action) { return HandleInput(actor as UnitScript, move, action); }
    public virtual IState HandleInput(FlyController actor, MoveInput move, ActionInput action) { return HandleInput(actor as UnitScript, move, action); }
    public virtual void OnEnter(UnitScript actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE) { }
    public virtual void OnEnter(PlayerController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE) { OnEnter(actor as UnitScript, move, action); }
    public virtual void OnEnter(ZombieController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE) { OnEnter(actor as UnitScript, move, action); }
    public virtual void OnEnter(ZombieBigController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE) { OnEnter(actor as UnitScript, move, action); }
    public virtual void OnEnter(FlyController actor, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE) { OnEnter(actor as UnitScript, move, action); }
    public virtual void StateUpdate(UnitScript actor) { }
    public virtual void StateUpdate(PlayerController actor) { StateUpdate(actor as UnitScript); }
    public virtual void StateUpdate(ZombieController actor) { StateUpdate(actor as UnitScript); }
    public virtual void StateUpdate(ZombieBigController actor) { StateUpdate(actor as UnitScript); }
    public virtual void StateUpdate(FlyController actor) { StateUpdate(actor as UnitScript); }
    public virtual string GetName() { return name; }
}
