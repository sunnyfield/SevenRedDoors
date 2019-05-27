using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : BaseState
{
    private const float fireRate = 0.35f;
    private const float zombieAttackRate = 1.3f;
    private const float zombieAttackDelay = 0.2f;
    private float delayTimer = 0f;
    private float timer = 0f;
    private new const string name = "Fire";

    public override void OnEnter(PlayerController player, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE)
    {
        if (player.TakeAmmo())
        {
            player.SetDrag(1000000f);
            player.Stop(true);
            player.Shoot();
            timer = fireRate;
            player.SetAnimation((int)AnimationState.ATTACK);    
        }
        else player.SetState(player.idleState);
    }

    public override void OnEnter(ZombieController zombie, MoveInput move = MoveInput.NONE, ActionInput action = ActionInput.NONE)
    {
        zombie.SetDrag(1000000f);
        zombie.Stop(true);
        delayTimer = zombieAttackDelay;
        timer = zombieAttackRate;
        zombie.SetAnimation((int)AnimationState.ATTACK);
    }

    public override void StateUpdate(PlayerController player)
    {
        if (timer > 0) timer -= Time.deltaTime;
        else player.SetState(player.idleState);
    }

    public override void StateUpdate(ZombieController zombie)
    {
        if (delayTimer > 0) delayTimer -= Time.deltaTime;
        else
        {
            delayTimer = zombieAttackRate;
            zombie.Attack();
            zombie.SetAnimation((int)AnimationState.IDLE);
        }

        if (timer > 0) timer -= Time.deltaTime;
        else zombie.SetState(zombie.idleState);

    }
}
