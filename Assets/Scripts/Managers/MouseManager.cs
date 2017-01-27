using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager instance;

    public event Action OnLeftMouseButtonUp;
    public event Action OnRightMouseButtonUp;

    [HideInInspector]
    public Vector2 WorldSpaceMousePosition;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseButtonUp();
        }

        if (Input.GetMouseButtonUp(1))
        {
            OnRightMouseButtonUp();
        }
        WorldSpaceMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
