using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehavior
{
    IBehavior StateUpdate(AIUnit actor);
    IBehavior StateUpdate(ZombieController actor);
    //IBehavior StateUpdate(ZombieBigController actor);
    //IBehavior StateUpdate(FlyController actor);
    void OnEnter(AIUnit actor);
    void OnEnter(ZombieController actor);
    //void OnEnter(ZombieBigController actor);
    //void OnEnter(FlyController actor);
}

public class BaseAIBehaviorState : IBehavior
{
    public static Transform target;
    protected Transform actorTransform;
    protected Vector2 vectorToTarget;
    protected MoveInput move = MoveInput.NONE;
    protected ActionInput action = ActionInput.NONE;

    protected bool initialized = false;

    public virtual IBehavior StateUpdate(AIUnit actor) { return null; }
    public virtual IBehavior StateUpdate(ZombieController actor) { return StateUpdate(actor as AIUnit); }
    //public virtual IBehavior StateUpdate(ZombieBigController actor) { return StateUpdate(actor as AIUnit); }
    //public virtual IBehavior StateUpdate(FlyController actor) { return StateUpdate(actor as AIUnit); }
    public virtual void OnEnter(AIUnit actor) { }
    public virtual void OnEnter(ZombieController actor) { OnEnter(actor as AIUnit); }
    //public virtual void OnEnter(ZombieBigController actor) { OnEnter(actor as AIUnit); }
    //public virtual void OnEnter(FlyController actor) { OnEnter(actor as AIUnit); }

    protected void VectorToTarget() { vectorToTarget = target.position - actorTransform.position; }
}
