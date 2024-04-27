using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : ISteering
{
    Transform _entity;
    Rigidbody2D _target;
    float _timePrediction;

    public Pursuit(Transform entity, Rigidbody2D target, float timePrediction)
    {
        _entity = entity;
        _target = target;
        _timePrediction = timePrediction;
    }
    public Vector3 GetDir()
    {
        Vector3 TargetPos = new Vector2(_target.position.x, _target.position.y);

        Vector2 point = TargetPos + _target.transform.up * _target.velocity.magnitude * _timePrediction;
        Vector2 dirToPoint = new Vector2 (point.x - _entity.position.x,point.y - _entity.position.y).normalized;
        Vector2 dirToTarget = (TargetPos - _entity.position).normalized;

        if (Vector3.Dot(dirToPoint, dirToTarget) < 0)
        {
            dirToPoint = dirToTarget;
#if UNITY_EDITOR
           // point = _target.position;// Debug
#endif
        }

#if UNITY_EDITOR
        //Debug.DrawRay(point, Vector3.up * 2, Color.red);// Debug
        //Debug.DrawRay(point, Quaternion.Euler(0, 0, 45) * Vector3.up * 2, Color.red);// Debug
        //Debug.DrawRay(point, Quaternion.Euler(0, 0, -45) * Vector3.up * 2, Color.red);// Debug
#endif
        return dirToPoint;
    }
}
