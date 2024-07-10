using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState<T> : State<T>
{
    Transform _player;
    Enemy _enemy;
    EnemyController _enemController;
    private float searchCooldown = 2f;

    public PlayerDetectedState(Enemy enemy, Transform player)
    {
        _player = player;
        _enemy = enemy;

    }
    public override void Enter()
    {
        base.Enter();
        _enemy.StopMovement();
    }
    public override void Execute()
    {
        base.Execute();
        _enemy.FollowSight(_player);
        
    }

    public override void Sleep()
    {
        base.Sleep();

        _enemy.ResumeMovement();
    }
}
