using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatePatrol<T> : State<T>
{
    #region Variables
    Enemy _enemy;
    [SerializeField] Transform[] _wayPoints;
    ObstacleAvoidance _obs;
    EnemyController _enemController;

    public int _currentWaypoint;
    #endregion

    #region Methods
    public EnemyStatePatrol(Enemy enemy, Transform[] Waypoints, ObstacleAvoidance Obs, EnemyController enemController)
    {
        _enemy = enemy;
        _wayPoints = Waypoints;
        _obs = Obs;
        _enemController = enemController;
    }

    public override void Execute()
    {
    
        //Get the current waypoint from the enemy Controller
        _currentWaypoint = _enemController.CurrentWaypoint();

        //Set a new transform to keep the direction
        Transform wp = _wayPoints[_currentWaypoint];

        //movement towards waypoint
        var dir = wp.position - _enemy.transform.position;
        //Use obstacle avoidance
        var dirNorm = _obs.GetDir(dir.normalized);

        _enemy.Move(dirNorm);
        _enemy.LookDir(dirNorm);


    }
    #endregion
}
