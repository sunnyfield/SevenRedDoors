using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : BaseAIBehaviorState
{
    float restRaitTimer;

    public override void OnEnter(ZombieController zombie)
    {
        if (actorTransform == null) actorTransform = zombie.transform;
        move = (MoveInput)(-actorTransform.right.x);
        restRaitTimer = Random.Range(1.5f, 3f);
    }

    public override IBehavior StateUpdate(ZombieController zombie)
    {
        VectorToTarget();
        if (vectorToTarget.x >= zombie.seeDistance)
        {
            if (restRaitTimer > 0)
            {
                if (actorTransform.position.x < zombie.leftBorder) move = MoveInput.RIGHT;
                else if (actorTransform.position.x > zombie.rightBorder) move = MoveInput.LEFT;
                restRaitTimer -= Time.deltaTime;

                zombie.HandleInput(move, ActionInput.NONE);
                return null;
            }
            else return zombie.restBehavior;
        }
        else return zombie.followBehavior;  
    }   
}
