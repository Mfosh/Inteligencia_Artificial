using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance
{
    float _angle;
    float _radius;
    Transform _entity;
    LayerMask _maskObs;
    float _personalArea;
    public ObstacleAvoidance(Transform entity, float angle, float radius, LayerMask maskObs,float  personalArea)
    {
        _angle = angle;
        _radius = radius;
        _entity = entity;
        _maskObs = maskObs;
        _personalArea = personalArea;
    }

    public Vector2 GetDir(Vector2 currentDir, bool calculateY = true)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(_entity.position, _radius, _maskObs);
        Collider2D nearColl = null;
        Vector2 closetPoint = Vector2.zero;
        float nearCollDistance = 0;
        if (!calculateY) currentDir.y = 0;
        for (int i = 0; i < colls.Length; i++)
        {
            var currentColl = colls[i];
            Debug.Log(currentColl);
            closetPoint = currentColl.ClosestPoint(_entity.position);
            if (!calculateY) closetPoint.y = _entity.position.y;
            Vector2 dirToColl = closetPoint - new Vector2 (_entity.position.x, _entity.position.y);
            float currentAngle = Vector2.Angle(dirToColl, currentDir);
            float distance = dirToColl.magnitude;

            if (currentAngle > _angle / 2) { continue; }

            if (nearColl == null)
            {
                nearColl = currentColl;
                nearCollDistance = distance;
                continue;
            }

            if (distance < nearCollDistance)
            {
                nearCollDistance = distance;
                nearColl = currentColl;
            }
        }
        if (nearColl == null)
        {

            return currentDir;
        }
        else
        {
            Vector2 relativePos = _entity.InverseTransformPoint(closetPoint);
            Vector2 dirToclosetPoint = (closetPoint - new Vector2(_entity.position.x,_entity.position.y )).normalized;
            Vector2 newDir;
            if (relativePos.x < 0)
            {
                newDir = Vector3.Cross(Vector3.back, dirToclosetPoint);
            }

            else
            {
                newDir = Vector3.Cross(Vector3.forward, dirToclosetPoint);
            }

            return Vector2.Lerp(currentDir, newDir, (_radius - Mathf.Clamp(nearCollDistance - _personalArea, 0, _radius)) / _radius);
        }


    }
}
