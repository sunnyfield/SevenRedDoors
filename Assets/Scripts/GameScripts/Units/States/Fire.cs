using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : IPlayerState
{
    private const float fireRate = 0.35f;
    private float timer = 0f;
    private const string name = "Fire";
    public string GetName()
    {
        return name;
    }
    public virtual void OnEnter(PlayerController player, MoveInput move, ActionInput action)
    {
            player.Shoot();
            timer = fireRate;
            player.SetAnimation((int)AnimationState.ATTACK);
    }

    public void StateUpdate(PlayerController player)
    {
        if (timer > 0) timer -= Time.deltaTime;
        else player.SetIdleState();
    }

    public void OnExit(PlayerController player)
    { }

    public virtual IPlayerState HandleInput(PlayerController player, MoveInput move, ActionInput action)
    {
        return null;
    }
}
