using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    bool isSwaying;
    public float swayAmount;
    public float swayTime;
    Vector3 originalPos;
    public void Update()
    {
        if (isSwaying)
        {
            Vector3 newPos = Random.insideUnitSphere * (Time.deltaTime * swayAmount);
            newPos.y = 0;
            transform.position = newPos + originalPos;
        }
    }
    public void SwayMe()
    {
        StartCoroutine("SwayProcessStart");
    }
    IEnumerator SwayProcessStart()
    {
        originalPos = transform.position;
        if (!isSwaying)
            isSwaying = true;
        yield return new WaitForSeconds(swayTime);
        isSwaying = false;
        transform.position = originalPos;
    }
}
