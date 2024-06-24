using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle<T> : State<T>
{
    T _inputMovement;
    public PlayerStateIdle(T inputMovement)
    {
        _inputMovement = inputMovement;
    }
    public override void Execute()
    {
        base.Execute();
        Debug.Log("Idle");
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        if (x != 0 || y != 0)
        {
            //Transition
            _fsm.Transition(_inputMovement);
        }
    }
}
