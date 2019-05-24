using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIBehavior
{
    PASSIVE,
    REST,
    FOLLOW,
    ATTACK
}

public class AIManager : MonoBehaviour
{
    public Transform target;

    [HideInInspector]
    static public List<ZombieController> zombies = new List<ZombieController>();
    //List<ZombieBigController> bigZombies = new List<ZombieBigController>();
    //List<FlyController> demons = new List<FlyController>();

    Vector2[] vectorsToTarget;
    MoveInput[] moveInputs;
    ActionInput[] actionInputs;


    const float zombieSeeDistance = 4f;
    const float zombieAttackDistance = 0.6f;

    void Start()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var enemy1 = transform.GetChild(i).GetComponent<ZombieController>();
            if (enemy1 != null) zombies.Add(enemy1);
            //else
            //{
            //    var enemy2 = transform.GetChild(i).GetComponent<ZombieBigController>();
            //    if (enemy2 != null) bigZombies.Add(enemy2);
            //    else
            //    {
            //        var enemy3 = transform.GetChild(i).GetComponent<FlyController>();
            //        if (enemy3 != null) demons.Add(enemy3);
            //    }
            //}
        }

        vectorsToTarget = new Vector2[zombies.Count];
        moveInputs = new MoveInput[zombies.Count];
        actionInputs = new ActionInput[zombies.Count];

        for(int i = 0; i < zombies.Count; i++)
        {
            moveInputs[i] = MoveInput.RIGHT;
            actionInputs[i] = ActionInput.NONE;
        }

        foreach(ZombieController zombie in zombies)
        {
            zombie.restRaitTimer = Random.Range(1.5f, 3f);
        }
    }

    void Update()
    {
        VectorToTargetUpdate();
        ZombieBehaviorUpdate();
        UpdateInput();
    }

    private void VectorToTargetUpdate()
    {
        int i = 0;
        foreach(ZombieController zombie in zombies)
        {
            if (zombie.gameObject.activeInHierarchy) vectorsToTarget[i] = target.position - zombie.transform.position;
            i++;
        }
    }

    private void UpdateInput()
    {
        int i = 0;
        foreach (ZombieController zombie in zombies)
        {
            if (zombie.gameObject.activeInHierarchy) zombie.HandleInput(moveInputs[i], actionInputs[i]);
            i++;
        }
    }

    private void ZombieBehaviorUpdate()
    {
        int zombieIndex = 0;
        foreach(ZombieController zombie in zombies)
        {
            if (zombie.gameObject.activeInHierarchy)
            {
                switch (zombie.behavior)
                {
                    case AIBehavior.PASSIVE:
                        if (vectorsToTarget[zombieIndex].magnitude < zombieSeeDistance)
                        {
                            if (zombie.transform.right.x * vectorsToTarget[zombieIndex].x < 0) moveInputs[zombieIndex] = (MoveInput)(-(int)moveInputs[zombieIndex]);
                            moveInputs[zombieIndex] = MoveInput.RIGHT;
                            zombie.behavior = AIBehavior.FOLLOW;
                        }
                        else
                        {
                            if (zombie.restRaitTimer > 0)
                            {
                                if (zombie.transform.position.x < zombie.leftBorder) moveInputs[zombieIndex] = MoveInput.RIGHT;
                                else if (zombie.transform.position.x > zombie.rightBorder) moveInputs[zombieIndex] = MoveInput.LEFT;
                                zombie.restRaitTimer -= Time.deltaTime;
                            }
                            else
                            {
                                zombie.restRaitTimer = Random.Range(1.5f, 3f);
                                zombie.restTimer = Random.Range(0.8f, 1.3f);
                                moveInputs[zombieIndex] = MoveInput.NONE;
                                zombie.behavior = AIBehavior.REST;
                            }
                        }
                        break;

                    case AIBehavior.ATTACK:
                        if (vectorsToTarget[zombieIndex].magnitude < zombieSeeDistance)
                        {
                            if (vectorsToTarget[zombieIndex].magnitude <= zombieAttackDistance)
                            {
                                if (zombie.transform.right.x * vectorsToTarget[zombieIndex].x < 0) zombie.Flip();
                            }
                            else
                            {
                                actionInputs[zombieIndex] = ActionInput.NONE;
                                moveInputs[zombieIndex] = (MoveInput)zombie.transform.right.x;
                                zombie.behavior = AIBehavior.FOLLOW;
                            }
                        }
                        else
                        {
                            if (zombie.transform.position.x < zombie.leftBorder) moveInputs[zombieIndex] = MoveInput.RIGHT;
                            else if (zombie.transform.position.x > zombie.rightBorder) moveInputs[zombieIndex] = MoveInput.LEFT;
                            else moveInputs[zombieIndex] = (MoveInput)zombie.transform.right.x;
                            zombie.behavior = AIBehavior.PASSIVE;
                        }
                        break;

                    case AIBehavior.FOLLOW:
                        if (vectorsToTarget[zombieIndex].magnitude < zombieSeeDistance)
                        {
                            if (vectorsToTarget[zombieIndex].magnitude > zombieAttackDistance)
                            {
                                if (zombie.transform.right.x * vectorsToTarget[zombieIndex].x < 0) moveInputs[zombieIndex] = (MoveInput)(-zombie.transform.right.x);
                                else if ((zombie.transform.position.x < zombie.leftBorder) && moveInputs[zombieIndex] < 0) moveInputs[zombieIndex] = MoveInput.NONE;
                                else if ((zombie.transform.position.x > zombie.rightBorder) && moveInputs[zombieIndex] > 0) moveInputs[zombieIndex] = MoveInput.NONE;
                            }
                            else
                            {
                                actionInputs[zombieIndex] = ActionInput.FIRE;
                                moveInputs[zombieIndex] = MoveInput.NONE;
                                zombie.behavior = AIBehavior.ATTACK;
                            }
                        }
                        else
                        {
                            if (zombie.transform.position.x < zombie.leftBorder) moveInputs[zombieIndex] = MoveInput.RIGHT;
                            else if (zombie.transform.position.x > zombie.rightBorder) moveInputs[zombieIndex] = MoveInput.LEFT;
                            else moveInputs[zombieIndex] = (MoveInput)zombie.transform.right.x;
                            zombie.behavior = AIBehavior.PASSIVE;
                        }
                        break;

                    case AIBehavior.REST:
                        if (zombie.restTimer > 0) zombie.restTimer -= Time.deltaTime;
                        else
                        {
                            zombie.restTimer = Random.Range(0.8f, 1.3f);
                            moveInputs[zombieIndex] = (MoveInput)(-zombie.transform.right.x);
                            zombie.behavior = AIBehavior.PASSIVE;
                        }
                        break;
                }
            }
            zombieIndex++;
        }   
    }
}
