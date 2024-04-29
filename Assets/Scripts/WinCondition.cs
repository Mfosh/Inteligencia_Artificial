using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinCondition : MonoBehaviour
{
    public GameObject _canvas;
    public TMP_Text _message;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            _message.text = "Ganaste!";
            _canvas.SetActive(true);
            Debug.Log("Win");
        }
    }
}
