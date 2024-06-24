using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWalk<T> : State<T>
{
    Player _player;
   
    T _idleInput;
    public PlayerStateWalk(Player player, T idleInput)
    {
        _player = player;
       
        _idleInput = idleInput;
    }
    public override void Execute()
    {
        base.Execute();
        Debug.Log("Walk");
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(x, y, 0).normalized;
        _player.Move(dir);
        _player.LookDir(dir);

        if (x == 0 && y == 0)
        {
            //Transition
            _fsm.Transition(_idleInput);
        }
    }
}
