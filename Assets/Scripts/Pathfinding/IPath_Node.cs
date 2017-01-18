using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPath_Node 
{
    IPath_Node[] GetNeighbors();
}