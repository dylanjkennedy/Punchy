using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupController : MonoBehaviour
{
    protected PickupSpawner.PickupType type;
    public float timer;
    protected float despawnRate = 20;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        timer = 0;
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
        timer += Time.deltaTime;
        //print(timer);

    }  


}
