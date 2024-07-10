using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Player, IBoid
{
    #region Variables
    public float AttackCooldown;
    public float _restTime;
    public float _resting = 5f;
    Coroutine _cooldown;
    Coroutine _rest;
    public bool isResting = true;

    public Action Shoot = delegate { };
    [SerializeField] private float _turnSpeed;

    public Vector3 Position => transform.position;
    public Vector3 Front => transform.position;

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

    public void FollowSight(Transform player)
    {
        Vector3 dir =  (player.position - transform.position).normalized;
        float atan2 = Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg - 90f);
    }
    public bool isCooldown => _cooldown != null;

    public bool EnemyResting => isResting;

    public void Rest(float duration)
    {
        _restTime = duration;
    }


    #endregion
}
