using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest : BaseAIBehaviorState
{
    float timer;

    public override void OnEnter(ZombieController zombie)
    {
        zombie.HandleInput(MoveInput.NONE, ActionInput.NONE);
        timer = Random.Range(0.8f, 1.3f);
    }

    public override IBehavior StateUpdate(ZombieController zombie)
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            return null;  
        }
        else return zombie.passiveBehavior;
    }
}
