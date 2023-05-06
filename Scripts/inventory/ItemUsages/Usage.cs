using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Usage : MonoBehaviour
{
    public virtual int Operate(Vector3 pos) { return 0; }
}
public abstract class Tool : Usage
{
    public int damage = 1;
    public float CD = 1;
    public WorldTimer LaborTimer;
    public bool TryRayHit(Vector3 pos,out GameObject hitObject)
    {
        float maxDistance = 1f;
        Vector3 rayStart = pos + new Vector3(0, -0.5f, 0); /*blockSelector.GetSelection()+new Vector3(0,0.25f,0)*/
        Vector3 rayDir = transform.up;
        Vector3 rayEnd = rayStart + rayDir * maxDistance;
        Ray ray = new Ray(rayStart, rayDir);
        Debug.DrawLine(rayStart, rayEnd, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            print(hit.point);
            print(hit.transform.position);
            print(hit.collider.gameObject);
            hitObject=hit.collider.gameObject;
            return true;
        }
        else 
        {
            hitObject = null;
            return false;
        }
    }
}
