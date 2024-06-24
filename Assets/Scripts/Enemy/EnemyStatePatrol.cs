using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatePatrol<T> : State<T>
{
    #region Variables
    Enemy _enemy;
    [SerializeField] List<Waypoints> _wayPoints;
    List<Waypoints> path;
    ObstacleAvoidance _obs;
    EnemyController _enemController;
    float radius = 500f;
    LayerMask maskObs;
    LayerMask maskWayP;
    int _nextPoint = 0;
    bool _isFinishPath = false;
    Waypoints Objective;

    #endregion

    #region Methods
    public EnemyStatePatrol(Enemy enemy, ObstacleAvoidance Obs, EnemyController enemController, LayerMask maskWaypoints, LayerMask Obstacles)
    {
        _enemy = enemy;

        _obs = Obs;
        _enemController = enemController;
        maskWayP = maskWaypoints;
        maskObs = Obstacles;
    
       

    }

    public override void Enter()
    {
        base.Enter();

        _nextPoint = 0;
        _isFinishPath = false;
        Objective = _enemController.GetObjective();
        var start = GetNearNode(_enemy.transform.position);
        Debug.Log(start);
        if (start == null)
        {
            return;
        }
        path = new List<Waypoints>();
        path = AStar.Run(start, GetConnections, IsSatiesfies, GetCost, Heuristic);

        path = AStar.CleanPath(path, InView);

    }

    public override void Execute()
    {

        //Get the current waypoint from the enemy Controller
     

        //Set a new transform to keep the direction
       // Transform wp = _wayPoints[_currentWaypoint].transform;

        //movement towards waypoint
       // var dir = wp.position - _enemy.transform.position;
        //Use obstacle avoidance
        //var dirNorm = _obs.GetDir(dir.normalized);


        FollowPath();
        //_enemy.GetStateWaypoints.SetWayPoints(path);
        //box.SetWayPoints(path);





    }
    #endregion

    void FollowPath() 
    {
        if (IsFinishPath) return;
        var point = path[_nextPoint].transform.position;
        Debug.Log(point);
        var posPoint = point;
        posPoint.z = _enemy.transform.position.z;

        Vector3 dir = posPoint - _enemy.transform.position;
       
        if (dir.magnitude < 0.2f)
        {
            if (_nextPoint + 1 < path.Count)
                _nextPoint++;
            else
            {
                _isFinishPath = true;
                _enemy.isResting = true;
                return;
            }
        }

       // var dirNorm = _obs.GetDir(dir.normalized);
        _enemy.Move(dir.normalized);
        _enemy.LookDir(dir);
    }



    Waypoints GetNearNode(Vector3 pos)
    {
        var waypoint = Physics2D.OverlapCircleAll(pos, radius, maskWayP);

        Waypoints nearWaypoint = null;
        float nearDistance = 0;
        for (int i = 0; i < waypoint.Length; i++)
        {
      
            var currentWaypoint = waypoint[i];
            var dir = currentWaypoint.transform.position - pos;
            float currentDistance = dir.magnitude;
            if (nearWaypoint == null || currentDistance < nearDistance)
            {

                if (!Physics.Raycast(pos, dir.normalized, currentDistance, maskObs))
                {
                 ;
                    nearWaypoint = currentWaypoint.GetComponent<Waypoints>();

                    nearDistance = currentDistance;

                }
            }
        }
     
        return nearWaypoint;
    }
    public bool IsFinishPath => _isFinishPath;

    List<Waypoints> GetConnections(Waypoints current)
    {
        return current.Connections;
    }

    bool IsSatiesfies(Waypoints current)
    {
        return current == Objective;
    }

    float GetCost(Waypoints parent, Waypoints child)
    {
        float cost = 0;
        float multiplierDistance = 1;

        cost += Vector3.Distance(parent.transform.position, child.transform.position) * multiplierDistance;

        return cost;
    }

    float Heuristic(Waypoints current)
    {
        if (Objective != null)
        {
            float heuristic = 0;
            float multiplierDistance = 1;
            heuristic += Vector3.Distance(current.transform.position, Objective.transform.position) * multiplierDistance;
            return heuristic;
        }
        else return 0;
    }

    bool InView(Waypoints grandParent, Waypoints child)
    {
        Vector3 dir = child.transform.position - grandParent.transform.position;
        Debug.Log(!Physics2D.Raycast(grandParent.transform.position, dir.normalized, dir.magnitude, maskObs));
        return !Physics2D.Raycast(grandParent.transform.position, dir.normalized, dir.magnitude, maskObs);

    }


}
