using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float scrollSpeed = 2f;

    private void Update()
    {
        Vector3 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // TODO: Create custom input manager
        Camera.main.transform.position += movement;
        Camera.main.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed * Camera.main.orthographicSize;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2, 15);
    }
}
