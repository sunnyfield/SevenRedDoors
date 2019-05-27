using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : BaseAIBehaviorState
{
    float restRaitTimer;

    public override void OnEnter(ZombieController zombie)
    {
        if (actorTransform == null) actorTransform = zombie.transform;
        zombie.SetPassiveSpeed();
        move = (MoveInput)(-actorTransform.right.x);
        restRaitTimer = Random.Range(2f, 5f);
    }

    public override IBehavior StateUpdate(ZombieController zombie)
    {
        VectorToTarget();
        if ((Mathf.Abs(vectorToTarget.x) >= zombie.seeDistance) || (Mathf.Abs(vectorToTarget.y) >= zombie.yThreshold))
        {
            if (restRaitTimer > 0)
            {
                Debug.DrawLine(actorTransform.localPosition, vectorToTarget + (Vector2)actorTransform.localPosition);
                if (actorTransform.localPosition.x <= zombie.leftBorder) move = MoveInput.RIGHT;
                else if (actorTransform.localPosition.x >= zombie.rightBorder) move = MoveInput.LEFT;
                restRaitTimer -= Time.deltaTime;

                zombie.HandleInput(move, ActionInput.NONE);
                return null;
            }
            else return zombie.restBehavior;
        }
        else return zombie.followBehavior;
    }   
}
