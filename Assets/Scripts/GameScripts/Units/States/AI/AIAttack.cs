using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttack : BaseAIBehaviorState
{
    public override void OnEnter(ZombieController zombie)
    {
        if (actorTransform == null) actorTransform = zombie.transform;
        zombie.HandleInput(MoveInput.NONE, ActionInput.FIRE);
    }

    public override IBehavior StateUpdate(ZombieController zombie)
    {
        VectorToTarget();
        if ((Mathf.Abs(vectorToTarget.x) <= zombie.attackDistance) && (Mathf.Abs(vectorToTarget.y) <= 0.1))
        {
            //Debug.DrawLine(actorTransform.localPosition, vectorToTarget + (Vector2)actorTransform.localPosition, Color.red);
            if (actorTransform.right.x * vectorToTarget.x < 0) zombie.Flip();
            zombie.HandleInput(MoveInput.NONE, ActionInput.FIRE);
            return null;
        }
        else return zombie.passiveBehavior;
    }
}
