using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    public float AttackCooldown;
    Coroutine _cooldown;

    public Action Shoot = delegate { };

    public void Attack()
    {
        _cooldown = StartCoroutine(Cooldown());
        Shoot();
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(AttackCooldown);
        _cooldown = null;
    
    }

    public bool isCooldown => _cooldown != null;


}
