using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    #region Variables
    public float AttackCooldown;
    Coroutine _cooldown;

    public Action Shoot = delegate { };
    #endregion

    #region Methods
    public void Attack()
    {
        //Start cooldown coroutine, when its finshed perform attack
        _cooldown = StartCoroutine(Cooldown());
        Shoot();
    }

    IEnumerator Cooldown()
    {
        //Wait for the duration of the cooldown variable and reset it
        yield return new WaitForSeconds(AttackCooldown);
        _cooldown = null;
    
    }

    public bool isCooldown => _cooldown != null;

    #endregion
}
