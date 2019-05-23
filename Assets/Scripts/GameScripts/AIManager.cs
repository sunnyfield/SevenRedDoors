using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIBehavior
{
    PASSIVE,
    AGRESSIVE,
    REST
}

public class AIManager : MonoBehaviour
{
    public Transform target;

    List<ZombieController> zombies = new List<ZombieController>();
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
        foreach(ZombieController zombie in zombies) { vectorsToTarget[i] = target.position - zombie.transform.position; /*print("vec to target " + i + " is " + vectorsToTarget[i]);*/ i++;  }
    }

    private void UpdateInput()
    {
        int i = 0;
        foreach (ZombieController zombie in zombies) { zombie.HandleInput(moveInputs[i], actionInputs[i]); print("move input " + moveInputs[i].ToString() + " action input " + actionInputs[i].ToString()); i++;}
    }

    private void ZombieBehaviorUpdate()
    {
        int zombieIndex = 0;
        foreach(ZombieController zombie in zombies)
        {
            zombie.restRaitTimer = Random.Range(1.5f, 3f);
            moveInputs[zombieIndex] = MoveInput.NONE;
            actionInputs[zombieIndex] = ActionInput.NONE;

            switch (zombie.behavior)
            {
                case AIBehavior.PASSIVE:
                    if (vectorsToTarget[zombieIndex].magnitude < zombieSeeDistance)
                    {
                        moveInputs[zombieIndex] = MoveInput.RIGHT;
                        zombie.behavior = AIBehavior.AGRESSIVE;
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

                            moveInputs[zombieIndex] = MoveInput.NONE;
                            zombie.behavior = AIBehavior.REST;
                        }
                    }
                    break;

                case AIBehavior.AGRESSIVE:
                    if (zombie.transform.right.x * vectorsToTarget[zombieIndex].x < 0) moveInputs[zombieIndex] = (MoveInput)(-(int)moveInputs[zombieIndex]);

                    if (vectorsToTarget[zombieIndex].magnitude < zombieSeeDistance)
                    {
                        if (vectorsToTarget[zombieIndex].magnitude <= zombieAttackDistance)
                        {
                            if (zombie.transform.right.x * vectorsToTarget[zombieIndex].x < 0) zombie.Flip();
                            moveInputs[zombieIndex] = MoveInput.NONE;
                            actionInputs[zombieIndex] = ActionInput.FIRE;
                        }
                        else
                        {
                            actionInputs[zombieIndex] = ActionInput.NONE;

                            if (zombie.transform.right.x * vectorsToTarget[zombieIndex].x < 0) moveInputs[zombieIndex] = (MoveInput)(-transform.right.x);
                            else if ((zombie.transform.position.x < zombie.leftBorder) && moveInputs[zombieIndex] < 0) moveInputs[zombieIndex] = MoveInput.NONE;
                            else if ((zombie.transform.position.x > zombie.rightBorder) && moveInputs[zombieIndex] > 0) moveInputs[zombieIndex] = MoveInput.NONE;
                        }
                    }
                    break;

                case AIBehavior.REST:
                    if (zombie.restTimer > 0) zombie.restTimer -= Time.deltaTime;
                    else
                    {
                        zombie.restTimer = Random.Range(0.8f, 1.3f);
                        zombie.behavior = AIBehavior.PASSIVE;
                    }
                    break;
            }
            zombieIndex++;
        }   
    }
}
