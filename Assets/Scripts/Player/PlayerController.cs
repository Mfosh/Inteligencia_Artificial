using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Player player;
    Vector2 MouseDirection;
    FSM<StatesEnum> _fsm;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        InitializeFSM();
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();

        var idle = new PlayerStateIdle<StatesEnum>(StatesEnum.Walk);
        var walk = new PlayerStateWalk<StatesEnum>(player, StatesEnum.Idle);

        idle.AddTransition(StatesEnum.Walk, walk);
        walk.AddTransition(StatesEnum.Idle, idle);

        _fsm.SetInit(idle);


    }
    // Update is called once per frame
    void Update()
    {
        _fsm.OnUpdate();
    }




    

    private Vector2 MouseDir()
    {
       Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5f));

        return mousePos;
    }
}
