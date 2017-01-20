using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPath_Node
{
    IPath_Node[] GetNeighbors();
    int X { get; set; }
    int Y { get; set; }
    /// <summary>
    /// Higher means  higher cost, more likely to travel through 0.8 than 1.2
    /// </summary>
    int MovementCost { get;} 

    Vector2 Position { get; set; }
}