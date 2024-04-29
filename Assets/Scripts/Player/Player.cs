using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region [Variables]
    [SerializeField] private float _movementSpeed;
    int _currentLife;
    int MaxLife = 5;
    private Rigidbody2D _rb;



    #endregion
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _currentLife = MaxLife;
    }


    void Update()
    {
        
    }
    
    
    public void Move(Vector3 Dir)
    {
        _rb.velocity = Dir * _movementSpeed;

    }

    public void LookDir(Vector3 dir)
    {
        if (dir.x == 0 && dir.y == 0) return;
        transform.up = dir;
    }

    public void TakeDamage(int damage)
    {
        _currentLife -= damage;
        Debug.Log(_currentLife);
        if(_currentLife <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Die");
    }


}
