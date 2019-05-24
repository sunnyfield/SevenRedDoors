using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZombieBehaviorState
{
    IZombieBehaviorState StateUpdate(ZombieController zombie);
    void OnEnter(ZombieController zombie);
}

public class Passive : IZombieBehaviorState
{
    static Transform target;
    Transform zombie;

    public IZombieBehaviorState StateUpdate(ZombieController zombie)
    {
        return null;
    }

    public void OnEnter(ZombieController zombieIn)
    {
        if (target == null) target = zombieIn.Target;
        if (zombie == null) zombie = zombieIn.transform;
    }
}
