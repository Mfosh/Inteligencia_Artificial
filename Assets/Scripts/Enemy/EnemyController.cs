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
    public float timePrediction;
    public float angle;
    public float radius;
    public LayerMask obsMask;
    ObstacleAvoidance _obstacleAvoidance;
    PlayerLineofSight _los;
    ITreeNode _root;
    public float attackRange;

    [SerializeField]float _patrolCooldown;
 

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
    }

    void InitializeSteeringsTest()
    {
        //Steering States & obstacle avoidance 
        var seek = new Seek(_enemy.transform, target.transform);
        var pursuit = new Pursuit(_enemy.transform, target, timePrediction);
        _steering = seek;
        _obstacleAvoidance = new ObstacleAvoidance(_enemy.transform, angle, radius, obsMask);
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();


        //States for the FSM 
        var idle = new EnemyStateIdle<StatesEnum>(_patrolCooldown, _enemy);
        var patrol = new EnemyStatePatrol<StatesEnum>(_enemy, _wayPoints, _obstacleAvoidance, this);
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
        var qIsResting = new QuestionNode(isEnemyResting, Idle, Patrol);
        var qNearWaypoint = new QuestionNode(NearWaypoint, qIsResting, Patrol);
        var qisCooldown = new QuestionNode(() => _enemy.isCooldown, Pursuit, Shoot);
        var qAttack = new QuestionNode(QuestionAttack, qisCooldown, Pursuit);
        var qLoS = new QuestionNode(QuestionLoS, qAttack , qNearWaypoint);

        //FirstQuestion
        _root = qLoS;
    }
    #endregion

    bool isEnemyResting()
    {
        if (_enemy.isResting)
        {
            Debug.Log("Beginrest");
            return true;
        }

        else { return false; }
    }

    bool QuestionLoS()
    {
        //Has the player been detected
        return _los.IsPlayerOnSight();
    }

    bool NearWaypoint()
    {
        //Check distance between NPC and Waypoints

        if (Vector2.Distance(_enemy.transform.position, _wayPoints[_currentWaypoint].transform.position) < 0.3f)
        {
            _enemy.isResting = true;
            //When near Wp, choose the next one
            NextWaypoint();
            Debug.Log("Reached Point");
           
            return true;
        }


        else
        {
            //Not reached waypoint Yet
            return false;
        }
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

    public int CurrentWaypoint()
    {
        //Return the current Waypoint. Utilized by patrol State to know wich waypoint is currently active
        return _currentWaypoint;
    }

    #endregion
}