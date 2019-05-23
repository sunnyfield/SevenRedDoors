using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : IPlayerState, IZombieState
{
    private const float fireRate = 0.35f;
    private const float zombieAttackRate = 1.3f;
    private const float zombieAttackDelay = 0.2f;
    private float delayTimer = 0f;
    private float timer = 0f;
    private const string name = "Fire";

    public string GetName() { return name; }

    public void OnEnter(PlayerController player, MoveInput move, ActionInput action)
    {
        if (player.TakeAmmo())
        {
            player.SetDrag(1000000f);
            player.Stop();
            player.Shoot();
            timer = fireRate;
            player.SetAnimation((int)AnimationState.ATTACK);    
        }
        else player.SetIdleState();
    }

    public void OnEnter(ZombieController zombie, MoveInput move, ActionInput action)
    {
        zombie.SetDrag(1000000f);
        zombie.Stop();
        delayTimer = zombieAttackDelay;
        timer = zombieAttackRate;
        zombie.SetAnimation((int)AnimationState.ATTACK);
    }

    public void StateUpdate(PlayerController player)
    {
        if (timer > 0) timer -= Time.deltaTime;
        else player.SetIdleState();
    }

    public void StateUpdate(ZombieController zombie)
    {
        if (delayTimer > 0) delayTimer -= Time.deltaTime;
        else
        {
            delayTimer = zombieAttackRate;
            zombie.Attack();
            zombie.SetAnimation((int)AnimationState.IDLE);
        }

        if (timer > 0) timer -= Time.deltaTime;
        else zombie.SetIdleState();

    }

    public virtual IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action) { return null; }

    public virtual IZombieState HandleInput(ZombieController zombie, MoveInput move, ActionInput action) { return null; }
}
