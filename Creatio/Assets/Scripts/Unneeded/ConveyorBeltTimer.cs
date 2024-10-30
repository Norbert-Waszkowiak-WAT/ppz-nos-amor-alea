using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltTimer : MonoBehaviour
{
    [SerializeField]
    private float interval;
    public UnityEngine.Events.UnityEvent OnBeltTick;

    private float timer;
    public float Interval 
    {
        get { return interval;}
        set { Interval = Mathf.Max(value, 0); }

    }


    // Start is called before the first frame update
    void Start()
    {
        timer = interval;
    }
//
    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime ;
        if( timer < 0 )
        {
            timer += interval ;
            if( OnBeltTick != null )
                OnBeltTick.Invoke();
        }
    }
}
