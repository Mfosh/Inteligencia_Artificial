using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateSteering<T> : State<T>
{
    ISteering _steering;
    Enemy _enemy;
    ObstacleAvoidance _obs;
    EnemyController _enemyController;

    public EnemyStateSteering(Enemy enemy, ISteering steering, ObstacleAvoidance obs, EnemyController enemyController)
    {
        _steering = steering;
        _enemy = enemy;
        _obs = obs;
        _enemyController = enemyController;
    }

    public override void Execute()
    {
        var dir = _obs.GetDir(_steering.GetDir());

        _enemy.Move(dir);
        _enemy.LookDir(dir);
    }

    public override void Sleep()
    {
        base.Sleep();
        _enemyController.LookingForPlayer = true;
        
    }
}
