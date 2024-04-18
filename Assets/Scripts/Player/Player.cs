using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region [Variables]
    [SerializeField] private float _movementSpeed;
    private Rigidbody2D _rb;


    #endregion
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }






    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector3 Dir, Vector2 mousePos)
    {
        _rb.velocity = Dir * _movementSpeed;

        Vector2 lookMouse = mousePos - _rb.position;
        float angle = Mathf.Atan2(lookMouse.y, lookMouse.x) * Mathf.Rad2Deg - 90f;
        _rb.rotation = angle;
    }


}
