using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatePatrol<T> : State<T>
{
    Enemy _enemy;
    [SerializeField] Transform[] _wayPoints;
    ObstacleAvoidance _obs;

    int _currentWaypoint;

    public EnemyStatePatrol(Enemy enemy, Transform[] Waypoints, ObstacleAvoidance Obs)
    {
        _enemy = enemy;
        _wayPoints = Waypoints;
        _obs = Obs;
    }

    public override void Execute()
    { 

        Transform wp = _wayPoints[_currentWaypoint];
        if (Vector2.Distance(_enemy.transform.position,wp.position) < 0.1f)
        {
            _currentWaypoint ++;
            if (_currentWaypoint >= _wayPoints.Length)  _currentWaypoint = 0;
        }
        else
        {
            var dir = wp.position- _enemy.transform.position;

            var dirNorm = _obs.GetDir(dir.normalized);

            _enemy.Move(dirNorm);
            _enemy.LookDir(dirNorm);
        }

    }




}
