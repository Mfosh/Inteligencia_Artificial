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
    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        InitializeSteerings();
        InitializeFSM();
        _rb = GetComponent<Rigidbody2D>();
    }

    void InitializeSteerings()
    {
        //var seek = new Seek(_magikarp.transform, target.transform);
        //var flee = new Flee(_magikarp.transform, target.transform);
        //var pursuit = new Pursuit(_magikarp.transform, target, timePrediction);
        //var evade = new Evade(_magikarp.transform, target, timePrediction);
        _steering = GetComponent<FlockingManager>();
        _obstacleAvoidance = new ObstacleAvoidance(_enemy.transform, angle, radius, obsMask, personalArea);
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();

        var idle = new EnemyStateIdle<StatesEnum>(_patrolCooldown, _enemy, _rb);
        var steering = new EnemyStateSteering<StatesEnum>(_enemy, _steering, _obstacleAvoidance);

        idle.AddTransition(StatesEnum.Walk, steering);
        steering.AddTransition(StatesEnum.Idle, idle);

        _fsm.SetInit(steering);
    }
    void Update()
    {
        _fsm.OnUpdate();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius);
    }
}