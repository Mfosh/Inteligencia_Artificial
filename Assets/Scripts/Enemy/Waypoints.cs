using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoints: MonoBehaviour 
{
    public WaypointsEnum type;
    public float probability;
    public List<Waypoints> Connections;
    
}
