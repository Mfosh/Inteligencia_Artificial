using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState<T> : State<T>
{
    Enemy _enemy;
    public EnemyAttackState(Enemy enemy)
    {
        _enemy = enemy;
    }

    public override void Execute()
    {
        base.Execute();
        _enemy.Attack();
    }
}
