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
    ObstacleAvoidance _obstacleAvoidance;
    PlayerLineofSight _los;
    ITreeNode _root;
    public float attackRange;
    public float personalArea = 5f;
    public float searchCooldown = 5f;
    [SerializeField]float _patrolCooldown;
    public Waypoints Objective;
    EnemyStatePatrol<StatesEnum> patrol;
    public bool LookingForPlayer;


    #region RWVariables
    Dictionary<WaypointsEnum, float> WaypointsDic;
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
        InitializeSteeringsTest();
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

    void InitializeSteeringsTest()
    {
        //Steering States & obstacle avoidance 
        var seek = new Seek(_enemy.transform, target.transform);
        var pursuit = new Pursuit(_enemy.transform, target, timePrediction);

        _steering = seek;
        Debug.Log(_steering);

        _obstacleAvoidance = new ObstacleAvoidance(_enemy.transform, angle, radius, obsMask, personalArea);
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();


        //States for the FSM 
        var idle = new EnemyStateIdle<StatesEnum>(_patrolCooldown, _enemy, _rb);
        patrol = new EnemyStatePatrol<StatesEnum>(_enemy,  _obstacleAvoidance, this, maskWayP, obsMask);
        var steering = new EnemyStateSteering<StatesEnum>(_enemy,_steering, _obstacleAvoidance);
        var shoot = new EnemyAttackState<StatesEnum>(_enemy);

        //Transitions between every state

        idle.AddTransition(StatesEnum.Walk, steering);
        idle.AddTransition(StatesEnum.Attack, shoot);
        idle.AddTransition(StatesEnum.Default, patrol);

        steering.AddTransition(StatesEnum.Idle, idle);
        steering.AddTransition(StatesEnum.Attack, shoot);
        steering.AddTransition(StatesEnum.Default, patrol);

        shoot.AddTransition(StatesEnum.Walk, steering);
        shoot.AddTransition(StatesEnum.Idle, idle);
        shoot.AddTransition(StatesEnum.Default, patrol);

        patrol.AddTransition(StatesEnum.Idle, idle);
        patrol.AddTransition(StatesEnum.Walk, steering);
        patrol.AddTransition(StatesEnum.Attack, shoot);



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


        //Questions

     
        var qIsResting = new QuestionNode(isEnemyResting, Idle,Patrol);
        var qisCooldown = new QuestionNode(() => _enemy.isCooldown, Pursuit, Shoot);
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
            searchCooldown = 5f;
        }
        return _los.IsPlayerOnSight();
    }

    bool NearWaypoint()
    {
  

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

        if (LookingForPlayer)
        {
            searchCooldown -= Time.deltaTime;
            if (searchCooldown <= 0)
            {
                LookingForPlayer = false;
            }
        }
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
        //Keep throwing the Roullette in case the new Waypoint is the same as the current one
        _currentWaypoint = (int)RouletteWheel.Roulette(WaypointsDic);
        while (Objective == wayPointsInfo[_currentWaypoint])
        {
            _currentWaypoint = (int)RouletteWheel.Roulette(WaypointsDic);
        }
        Objective = wayPointsInfo[_currentWaypoint];
        return _currentWaypoint;
    }

    #endregion
}