using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : BaseAIBehaviorState
{
    public override void OnEnter(ZombieController zombie)
    {
        if (actorTransform == null) actorTransform = zombie.transform;
        zombie.SetFollowSpeed();
        VectorToTarget();
        if (actorTransform.right.x * vectorToTarget.x < 0) move = (MoveInput)(-actorTransform.right.x);
        else move = (MoveInput)actorTransform.right.x;
    }

    public override IBehavior StateUpdate(ZombieController zombie)
    {
        VectorToTarget();
        float vectorAbsX = Mathf.Abs(vectorToTarget.x);
        if ((vectorAbsX < zombie.seeDistance) && (vectorToTarget.y > zombie.yThresholdBottom && vectorToTarget.y < zombie.yThresholdTop))
        {
            if (vectorAbsX > zombie.attackDistance || Mathf.Abs(vectorToTarget.y) > 0.1)
            {
                //Debug.DrawLine(actorTransform.localPosition, vectorToTarget + (Vector2)actorTransform.localPosition, Color.yellow);
                if (vectorAbsX > 0.1)
                {
                    move = (MoveInput)actorTransform.right.x;
                    if (actorTransform.right.x * vectorToTarget.x < 0) move = (MoveInput)(-actorTransform.right.x);
                    else if (actorTransform.localPosition.x <= zombie.leftBorder && move < 0) move = MoveInput.NONE;
                    else if (actorTransform.localPosition.x >= zombie.rightBorder && move > 0) move = MoveInput.NONE;
                }
                else move = MoveInput.NONE;

                zombie.HandleInput(move, ActionInput.NONE);
                return null;
            }
            else return zombie.aIAttack;
        }
        else return zombie.passiveBehavior;
    }
}
