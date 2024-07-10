using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyFlocking : MonoBehaviour
{
    public Rigidbody2D target;
    public float timePrediction;
    public float angle;
    public float radius;
    float personalArea = 5f;
    public float _patrolCooldown;
    public LayerMask obsMask;
    FSM<StatesEnum> _fsm;
    ISteering _steering;
    Enemy _enemy;
    ObstacleAvoidance _obstacleAvoidance;
    Rigidbody2D _rb;
    public Rigidbody2D playerPos;
    ITreeNode _root;
    PlayerLineofSight _los;
    float attackRange;
    [SerializeField] float minDistance;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        InitializeSteerings();
        InitializeFSM();
        InitializeTree();
        _los = GetComponent<PlayerLineofSight>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void InitializeSteerings()
    {

        _steering = GetComponent<FlockingManager>();
        _obstacleAvoidance = new ObstacleAvoidance(_enemy.transform, angle, radius, obsMask, personalArea);
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();


        var steering = new EnemyStateSteering<StatesEnum>(_enemy, _steering, _obstacleAvoidance);
        var shoot = new EnemyAttackState<StatesEnum>(_enemy);
        var follow = new PlayerDetectedState<StatesEnum>(_enemy, playerPos.transform);

        steering.AddTransition(StatesEnum.Attack, shoot);
        steering.AddTransition(StatesEnum.Chase, follow);

        shoot.AddTransition(StatesEnum.Default, steering);
        shoot.AddTransition(StatesEnum.Chase, follow);

        follow.AddTransition(StatesEnum.Default, steering);
        follow.AddTransition(StatesEnum.Attack, shoot);
        _fsm.SetInit(steering);
    }

    void InitializeTree()
    {

        //Desicion tree Initializacion

        //Actions to perform
        var Pursuit = new ActionNode(() => _fsm.Transition(StatesEnum.Walk));
        var Shoot = new ActionNode(() => _fsm.Transition(StatesEnum.Attack));
        var Patrol = new ActionNode(() => _fsm.Transition(StatesEnum.Default));
        var Watch = new ActionNode(() => _fsm.Transition(StatesEnum.Chase));
        //Questions

 
      
        var qCheckDistance = new QuestionNode(CheckDistance, Watch, Pursuit );
        var qisCooldown = new QuestionNode(() => _enemy.isCooldown, qCheckDistance, Shoot);
        var qAttack = new QuestionNode(QuestionAttack, qisCooldown, Pursuit);
        var qLoS = new QuestionNode(QuestionLoS, qAttack, Patrol);

        //FirstQuestion
        _root = qLoS;
    }


    void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
    }


    bool QuestionLoS()
    {
        return _los.IsPlayerOnSight();
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

    bool CheckDistance()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, minDistance);
        foreach (var col in cols)
        {
            if (col.GetComponent<Player>())
            {
                return true;
            }
        }

        return false;
    }







    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius);
    }
}