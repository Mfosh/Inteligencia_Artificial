using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTree : MonoBehaviour
{
   [SerializeField] bool playerDetected = false;
   [SerializeField] int life = 100;
   [SerializeField] public bool los;
   [SerializeField] ITreeNode _root;

    private void Awake()
    {
        InitializeTree();
    }
    
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _root.Execute();
        }
    }

    void InitializeTree()
    {
        var patrol = new ActionNode(() => print("Patrol"));
        var follow = new ActionNode(() => print("Following"));
        var attack = new ActionNode(() => print("Attack"));
        var die = new ActionNode(() => print("Dead"));
        ITreeNode qLoS = new QuestionNode(QuestionLoS, follow, patrol);

        ITreeNode qHaslife = new QuestionNode(()=> life > 0, qLoS, die);



        _root = qHaslife;
    }

    public bool QuestionLoS()
    {
        return playerDetected;
    }



}
