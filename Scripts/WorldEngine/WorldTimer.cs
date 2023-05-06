using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTimer
{
    float StartedTime;
    public WorldTimer(){
        StartedTime = Time.time;
    }
    public void Capture()
    {
        StartedTime = Time.time;
    }
    public bool Elapsed(float Duration)
    {
        return (Time.time - StartedTime) > Duration;
    }
    public float GetElapsedTime()
    {
        return (Time.time - StartedTime);
    }
}
