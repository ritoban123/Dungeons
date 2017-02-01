using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager instance;

    public event Action OnLeftMouseButtonUp;
    public event Action OnRightMouseButtonUp;
    public event Action<int, GameObject> OnMouseOverLayer;

    [HideInInspector]
    public Vector2 WorldSpaceMousePosition;
    [HideInInspector]
    public GameObject GameObjectUnderMouse;

    private void Awake()
    {
        if (instance != null)
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
            if(OnLeftMouseButtonUp != null)
                OnLeftMouseButtonUp();
        }

        if (Input.GetMouseButtonUp(1))
        {
            if(OnRightMouseButtonUp != null)
                OnRightMouseButtonUp();
        }
        HandleGameObjectUnderMouse();
    }

    private void HandleGameObjectUnderMouse()
    {
        WorldSpaceMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(WorldSpaceMousePosition, Camera.main.transform.forward);
        if (hitInfo.collider != null)
        {
            if(OnMouseOverLayer != null)
                OnMouseOverLayer(hitInfo.collider.gameObject.layer, hitInfo.collider.gameObject);
            GameObjectUnderMouse = hitInfo.collider.gameObject;
        }
        else
        {
            if(OnMouseOverLayer != null)
                OnMouseOverLayer(0, null);

            GameObjectUnderMouse = null;
        }
    }
}
