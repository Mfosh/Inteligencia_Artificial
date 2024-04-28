using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    Enemy enemy;
    [SerializeField] int _ammo;
    public GameObject Bullet;

    public Transform shootingPoint;

    private void Awake()
    {
       enemy = GetComponent<Enemy>();
        enemy.Shoot += Shoot;
    }

     void Shoot()
    {
        Instantiate(Bullet, shootingPoint.position, shootingPoint.rotation);
    }
}
