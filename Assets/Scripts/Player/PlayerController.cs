using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Player player;
    Vector2 MouseDirection;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {


        MouseDirection = MouseDir();

        float x = Input.GetAxis("Horizontal");
         float y = Input.GetAxis("Vertical");
         Vector3 dir = new Vector3(x, y, 0).normalized;

        player.Move(dir);
        player.LookDir(dir);



    }



    private Vector2 MouseDir()
    {
       Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5f));

        return mousePos;
    }
}
