using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
   // [SerializeField] public Transform triangulo;

    [SerializeField] private float distancia;

    public Vector3 puntoInicial;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        puntoInicial = transform.position;

    }

    private void Update()
    {
       // distancia = Vector2.Distance(transform.position, triangulo.position);
        animator.SetFloat("Distancia", distancia);
    }


}
