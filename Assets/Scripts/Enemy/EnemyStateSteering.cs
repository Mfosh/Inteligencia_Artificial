using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateSteering<T> : State<T>
{
    ISteering _steering;
    Enemy _enemy;

    public EnemyStateSteering(Enemy enemy, ISteering steering)
    {
        _steering = steering;
        _enemy = enemy;
    }

    public override void Execute()
    {
        var dir = _steering.GetDir();
        _enemy.Move(dir);
        _enemy.LookDir(dir);
    }
}
