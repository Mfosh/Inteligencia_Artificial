using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player
{
    #region Variables
    public float AttackCooldown;
    public float _restTime;
    Coroutine _cooldown;
    Coroutine _rest;

    public bool isResting;

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


    private void Update()
    {
        _restTime -= Time.deltaTime;

        if(_restTime <= 0)
        {
            isResting = false;
        }
    }


    public bool isCooldown => _cooldown != null;

    public bool EnemyResting => isResting;

    public void Rest(float duration)
    {
        _restTime = duration;
    }


    #endregion
}
