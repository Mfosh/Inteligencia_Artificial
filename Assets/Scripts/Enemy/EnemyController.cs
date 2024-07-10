using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Variables

    FSM<StatesEnum> _fsm;
    ISteering _steering;
    Enemy _enemy;
    public Rigidbody2D target;
    Rigidbody2D _rb;
    public float timePrediction;
    public float angle;
    public float radius;
    public LayerMask obsMask;
    public LayerMask maskWayP;
    public LayerMask maskObsWalls;
    ObstacleAvoidance _obstacleAvoidance;
    PlayerLineofSight _los;
    ITreeNode _root;
    public float attackRange;
    public float personalArea = 5f;
    public float searchCooldown = 5f;
    [SerializeField] float minDistance;
    [SerializeField]float _patrolCooldown;
    public Waypoints Objective;
    EnemyStatePatrol<StatesEnum> patrol;
    public bool LookingForPlayer;


    #region RWVariables
    Dictionary<WaypointsEnum, float> WaypointsDic;
    Dictionary<WaypointsEnum, float> RouletteDic;
    public List<Waypoints> wayPointsInfo;
    [SerializeField] Transform[] _wayPoints;
    [SerializeField] int _currentWaypoint;
    #endregion

    #endregion

    #region Methods

    #region Initializations
    private void Awake()
    {
        //Initialize Components
        _enemy = GetComponent<Enemy>();
        _los = GetComponent<PlayerLineofSight>();
        _rb = GetComponent<Rigidbody2D>();
        InitializeSteerings();
        InitializeFSM();
        InitializeTree();

        //Diccionary creation for RW
        WaypointsDic = new Dictionary<WaypointsEnum, float>();
        for (int i = 0; i < wayPointsInfo.Count; i++)
        {
            var curr = wayPointsInfo[i];
            WaypointsDic[curr.type] = curr.probability;
        }


        Debug.Log(_fsm.CurrentState.ToString());

        CurrentWaypoint();
    }

    void InitializeSteerings()
    {
        //Steering States & obstacle avoidance 
        //var seek = new Seek(_enemy.transform, target.transform);
        var pursuit = new Pursuit(_enemy.transform, target, timePrediction);

        _steering = pursuit;
        Debug.Log(_steering);

        _obstacleAvoidance = new ObstacleAvoidance(_enemy.transform, angle, radius, obsMask, personalArea);
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();


        //States for the FSM 
        var idle = new EnemyStateIdle<StatesEnum>(_patrolCooldown, _enemy, _rb);
        patrol = new EnemyStatePatrol<StatesEnum>(_enemy,  _obstacleAvoidance, this, maskWayP, maskObsWalls);
        var steering = new EnemyStateSteering<StatesEnum>(_enemy,_steering, _obstacleAvoidance);
        var attack = new EnemyAttackState<StatesEnum>(_enemy);
        var watch = new PlayerDetectedState<StatesEnum>(_enemy, target.transform);
        //Transitions between every state

        idle.AddTransition(StatesEnum.Walk, steering);
        idle.AddTransition(StatesEnum.Attack, attack);
        idle.AddTransition(StatesEnum.Default, patrol);
        idle.AddTransition(StatesEnum.Chase, watch);

        steering.AddTransition(StatesEnum.Idle, idle);
        steering.AddTransition(StatesEnum.Attack, attack);
        steering.AddTransition(StatesEnum.Default, patrol);
        steering.AddTransition(StatesEnum.Chase, watch);

        attack.AddTransition(StatesEnum.Walk, steering);
        attack.AddTransition(StatesEnum.Idle, idle);
        attack.AddTransition(StatesEnum.Default, patrol);
        attack.AddTransition(StatesEnum.Chase, watch);

        patrol.AddTransition(StatesEnum.Idle, idle);
        patrol.AddTransition(StatesEnum.Walk, steering);
        patrol.AddTransition(StatesEnum.Attack, attack);
        patrol.AddTransition(StatesEnum.Chase, watch);

        watch.AddTransition(StatesEnum.Idle, idle);
        watch.AddTransition(StatesEnum.Walk, steering);
        watch.AddTransition(StatesEnum.Attack, attack);
        watch.AddTransition(StatesEnum.Default, patrol);
        //Estado Inicial
        _fsm.SetInit(idle);
    }


    void InitializeTree()
    {

        //Desicion tree Initializacion

        //Actions to perform
        var Pursuit = new ActionNode(() => _fsm.Transition(StatesEnum.Walk));
        var Idle = new ActionNode(() => _fsm.Transition(StatesEnum.Idle));
        var Shoot = new ActionNode(() => _fsm.Transition(StatesEnum.Attack));
        var Patrol = new ActionNode(() => _fsm.Transition(StatesEnum.Default));
        var Watch = new ActionNode(() => _fsm.Transition(StatesEnum.Chase));

        //Questions

     
        var qIsResting = new QuestionNode(isEnemyResting, Idle,Patrol);
        var qStillInSight = new QuestionNode(QuestionLoS, Pursuit, Patrol);
        var qCheckDistance = new QuestionNode(CheckDistance, Watch, qStillInSight);
        var qisCooldown = new QuestionNode(() => _enemy.isCooldown, qCheckDistance, Shoot);
        var qAttack = new QuestionNode(QuestionAttack, qisCooldown, Pursuit);
        var qLoS = new QuestionNode(QuestionLoS, qAttack ,qIsResting);

        //FirstQuestion
        _root = qLoS;
    }
    #endregion

    bool isPathFinished()
    {
        Debug.Log(patrol.IsFinishPath);
        return patrol.IsFinishPath;
    }

    bool isEnemyResting()
    {
        return _enemy.isResting;
    }

    bool QuestionLoS()
    {
        var playerDetected = _los.IsPlayerOnSight();
        if (playerDetected)
        {
            LookingForPlayer = true;

        }
        return playerDetected;
    }



    bool CheckDistance()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, minDistance);
        foreach (var col in cols)
        {
            if (col.transform.GetComponent<Player>())
            {
                Debug.Log("Player Detected");
                return true;
            }
        }

        return false;
    }



    bool QuestionAttack()
    {
        //Check distance for attacks

        bool canAttack = false;
        Collider2D[] collider2s = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var cols in collider2s)
        {
            if (cols.transform.GetComponent<Player>())
            {
                //Look in the near collisions for the player
                canAttack = true;
                break;
            }
            
        }
        if (_los.IsPlayerOnSight() && canAttack)
            return true;
        else
            return false;
    }

    void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();

   
    }


    void NextWaypoint()
    {
        //Randomly choose the next Waypoint from the dictionary & List
       _currentWaypoint = (int)RouletteWheel.Roulette(WaypointsDic);
    }

    public Waypoints GetObjective()
    {
        CurrentWaypoint();
        return Objective;
    }

    public int CurrentWaypoint()
    {

        //Set all Waypoints to a low probability
        for (int i = 0; i < wayPointsInfo.Count; i++)
        {
            wayPointsInfo[i].probability = 10;
        }

        if (LookingForPlayer)
        {
            var nearWaypoints = Physics2D.OverlapCircleAll(target.position, 2f, maskWayP);
            for (int i = 0; i < nearWaypoints.Length; i++)
            {
                var waypoint = nearWaypoints[i].GetComponent<Waypoints>();
                waypoint.probability = 100;
            }
        }
        else
        {
            //Check for near WP and increase their probabilites
            var nearWaypoints = Physics2D.OverlapCircleAll(transform.position, 5f, maskWayP);
            for (int i = 0; i < nearWaypoints.Length; i++)
            {
                if (nearWaypoints[i].GetComponent<Waypoints>())
                {
                    var waypoint = nearWaypoints[i].GetComponent<Waypoints>();
                    waypoint.probability = 70;
                }
            }
        }

        //Create a new dictionary without the current objective to avoid repetition
        RouletteDic = new Dictionary<WaypointsEnum, float>();
        for (int i = 0; i < wayPointsInfo.Count; i++)
        {
            if (i == _currentWaypoint)
            {
                continue;
            }
            var curr = wayPointsInfo[i];
            RouletteDic[curr.type] = curr.probability;
        }


        _currentWaypoint = (int)RouletteWheel.Roulette(RouletteDic);
        Objective = wayPointsInfo[_currentWaypoint];
        return _currentWaypoint;
    }

    #endregion
}