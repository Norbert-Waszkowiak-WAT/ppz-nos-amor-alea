using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float zoomSpeed = 4f;
    public float minZoom = 3f;
    public float maxZoom = 25f;

    void Update()
    {
        transform.position = target.position + offset;

        ScrollZoom();

        
    }

    void ScrollZoom() {
        float currentZoom = Camera.main.orthographicSize;
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(currentZoom >= minZoom && currentZoom <= maxZoom) {
            Camera.main.orthographicSize -= scroll * zoomSpeed;
        } else if(currentZoom < minZoom) {
            Camera.main.orthographicSize =  minZoom;
        } else if(currentZoom > maxZoom) {
            Camera.main.orthographicSize = maxZoom;
        }
    }
}
