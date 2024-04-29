using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateIdle<T> : State<T>
{
    float _patrolCooldown;
    float _cooldown;
    Coroutine _rest;

    Enemy _enemy;

    public EnemyStateIdle(float Cooldown, Enemy enemy)
    {
        _cooldown = Cooldown;

        _patrolCooldown = Cooldown;
        Debug.Log(_patrolCooldown);
        _enemy = enemy;
    }

    public override void Execute()
    {

        Rest();
 
  
    }

    IEnumerator Rest()
    {
        //Wait for the duration of the cooldown variable and reset it
        yield return new WaitForSeconds(_patrolCooldown);
        _rest = null;

    }
}
