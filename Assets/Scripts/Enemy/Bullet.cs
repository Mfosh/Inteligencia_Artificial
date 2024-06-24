using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), (typeof(Rigidbody2D)))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private LayerMask _hittableLayers;
    [SerializeField] private int _damage;


    private Collider2D _collider;
    private Rigidbody2D _rigidbody;
    // Start is called before the first frame update
    void Start()
    {

      _collider = GetComponent<Collider2D>();
      _rigidbody = GetComponent<Rigidbody2D>();
      Initialize();

    }

    void Update()
    {
        Travel();
    }
    public void Travel()
    {
        transform.position += transform.up * Time.deltaTime * _speed;
    }




    public void Initialize()
    {
        _collider.isTrigger = true;
        _rigidbody.isKinematic = true;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }


    // Update is called once per frame


    public void OnTriggerEnter2D(Collider2D other)
    {

        if (((1 << other.gameObject.layer) & _hittableLayers) != 0)
        {
            other.GetComponent<Player>()?.TakeDamage(_damage);
            Destroy(this.gameObject);
        }
    }



}
