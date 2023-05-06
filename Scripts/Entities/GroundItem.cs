using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public ItemObject item;
    public WorldTimer timer;
    private void Start()
    {
        timer = new WorldTimer();
        InvokeRepeating("DetectFalling", 0.5f, 0.5f);
    }
    public void DetectFalling()
    {
        //Debug.LogWarning("Detecting");
        if (transform.position.y < -2)
        {
            //Debug.LogWarning("Deleted");
            Destroy(gameObject);
        }
    }
}
