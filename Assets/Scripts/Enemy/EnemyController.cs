using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
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

    [SerializeField] Transform[] _wayPoints;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _los = GetComponent<PlayerLineofSight>();
        InitializeSteeringsTest();
        InitializeFSM();
        InitializeTree();

        Debug.Log(_fsm.CurrentState.ToString());
    }

    void InitializeSteeringsTest()
    {
        var seek = new Seek(_enemy.transform, target.transform);
        var pursuit = new Pursuit(_enemy.transform, target, timePrediction);
        _steering = seek;
        _obstacleAvoidance = new ObstacleAvoidance(_enemy.transform, angle, radius, obsMask);
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();



        var idle = new EnemyStateIdle<StatesEnum>();
        var patrol = new EnemyStatePatrol<StatesEnum>(_enemy, _wayPoints, _obstacleAvoidance);
        var steering = new EnemyStateSteering<StatesEnum>(_enemy,_steering, _obstacleAvoidance);
        var shoot = new EnemyAttackState<StatesEnum>(_enemy);


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
        var Pursuit = new ActionNode(() => _fsm.Transition(StatesEnum.Walk));
        var Idle = new ActionNode(() => _fsm.Transition(StatesEnum.Idle));
        var Shoot = new ActionNode(() => _fsm.Transition(StatesEnum.Attack));
        var Patrol = new ActionNode(() => _fsm.Transition(StatesEnum.Default));

        var qisCooldown = new QuestionNode(() => _enemy.isCooldown, Pursuit, Shoot);
        var qAttack = new QuestionNode(QuestionAttack, qisCooldown, Pursuit);
        var qLoS = new QuestionNode(QuestionLoS, qAttack , Patrol);


        _root = qLoS;
    }

    bool QuestionLoS()
    {
        return _los.IsPlayerOnSight();
    }

    bool QuestionAttack()
    {
        bool canAttack = false;
        Collider2D[] collider2s = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var cols in collider2s)
        {
            if (cols.transform.GetComponent<Player>())
            {
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
}
