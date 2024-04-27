using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{

    [SerializeField] float _rotationSpeed;
    PlayerController playerController;


    // Start is called before the first frame update
    void Start()
    {
       playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
  
        
        
       
    }

    private void FixedUpdate()
    {
        //PointAtMouse();
    }

    public void FollowMouse(Vector2 mousePos)
    {

    }

}


