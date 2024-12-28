using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallas : MonoBehaviour
{
    private float startpos, lenght;
    public GameObject cam;
    public float parallaxEffect;
    public float moveSpeed = 1f; 

    void Start()
    {
        startpos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        
        startpos += moveSpeed * Time.fixedDeltaTime; 

        if (temp > startpos + lenght) startpos += lenght;
        else if (temp < startpos - lenght) startpos -= lenght;
    }
}
