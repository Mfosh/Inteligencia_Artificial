using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    FSM<StatesEnum> _fsm;
    ISteering _steering;
    Enemy _enemy;
    public Rigidbody target;
    public float timePrediction;
    public float angle;
    public float radius;
    public LayerMask obsMask;
    ObstacleAvoidance _obstacleAvoidance;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        InitializeSteeringsTest();
        InitializeFSM();
    }

    void InitializeSteeringsTest()
    {
        var seek = new Seek(_enemy.transform, target.transform);
        var pursuit = new Pursuit(_enemy.transform, target, timePrediction);
        _steering = pursuit;
        _obstacleAvoidance = new ObstacleAvoidance(_enemy.transform, angle, radius, obsMask);
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();



        var idle = new EnemyStateIdle<StatesEnum>();
        var steering = new EnemyStateSteering<StatesEnum>(_enemy,_steering, _obstacleAvoidance);

        idle.AddTransition(StatesEnum.Walk, steering);
        steering.AddTransition(StatesEnum.Idle, idle);

        //Estado Inicial
        _fsm.SetInit(steering);
    }
    void Update()
    {
        _fsm.OnUpdate();
    }
}
