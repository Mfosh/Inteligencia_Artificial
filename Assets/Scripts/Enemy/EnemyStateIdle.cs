using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateIdle<T> : State<T>
{
    float _patrolCooldown;
    float _cooldown;
    Coroutine _rest;
    Rigidbody2D rb;
    Enemy _enemy;

    public EnemyStateIdle(float Cooldown, Enemy enemy, Rigidbody2D _rb)
    {
        _cooldown = Cooldown;

        _patrolCooldown = Cooldown;
        Debug.Log(_patrolCooldown);
        rb = _rb;
        _enemy = enemy;
    }

    public override void Enter()
    {
        _enemy._restTime = _cooldown;
        rb.bodyType = RigidbodyType2D.Static;
    }
    public override void Execute()
    {

 
  
    }

    public override void Sleep()
    {
        base.Sleep();
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}
