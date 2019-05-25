using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : UnitScript
{
    public Vector2 vectorToTarget;
    public float seeDistance;
    public float attackDistance;
    public float leftBorder;
    public float rightBorder;
    public LayerMask whatToHit;
    public readonly Passive passiveBehavior = new Passive();
    public readonly Rest restBehavior = new Rest();
    public readonly Follow followBehavior = new Follow();
}
