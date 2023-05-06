using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SickleEntity : MonoBehaviour
{
    public int damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Crop crop))
        {
            crop.Harm(damage);
        }
        else if(other.TryGetComponent(out Grass grass))
        {
            grass.Harm(damage);
        }
        else if (other.TryGetComponent(out Bush HittedBush))
        {
            HittedBush.Harvest();
        }
    }
}
