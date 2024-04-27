using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLineofSight : MonoBehaviour
{
    // El rango maximo de deteccion
    public float Range;
    // Las layers que son consideradas jugador
    public LayerMask WhatIsPlayer;
    // Las layers que bloquean la vision
    public LayerMask WhatBlocksSight;

    // Update es llamado una vez por frame
    void Update ()
    {
        // Llamamos el metodo
        IsPlayerOnSight();
    }

    public bool IsPlayerOnSight()
    {
        // Declaramos la variable y conseguimos informacion de si el jugador esta en rango o no
        var playerOnRange = Physics2D.OverlapCircleAll(transform.position, Range, WhatIsPlayer);
        
        // Si el jugador no esta en rango devolvemos false
        if (playerOnRange == null || playerOnRange.Length <= 0) return false; 

        // Lanzamos un rayo hacia el jugador para detectar si hay algo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerOnRange[0].transform.position - transform.position, Range, WhatBlocksSight); 
       
        // Si el rayo detecta algo seguimos con el codigo
        if(hit.collider != null)
        {
            // Revisamos si lo que colisiono es el jugador o es otro objeto
            if (hit.transform == playerOnRange[0].transform)
            {
                Debug.Log("Se encontro un jugador");
                return true;
            }
        }
        // No colisionamos con el jugador, por lo tanto no se encontro ningun jugador
        return false;
    }
}
