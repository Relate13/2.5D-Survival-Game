using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    bool isShaking;
    public float shakeAmount;
    public float shakeTime;
    Vector3 originalPos;
    public void Update()
    {
        if(isShaking)
        {
            Vector3 newPos = Random.insideUnitSphere * (Time.deltaTime * shakeAmount);
            newPos.y = 0;
            transform.position = newPos + originalPos;
        }   
    }
    public void ShakeMe()
    {
        if(!isShaking)
            StartCoroutine("ShakeProcessStart");
    }
    IEnumerator ShakeProcessStart()
    {
        originalPos = transform.position;
        if (!isShaking)
            isShaking = true;
        yield return new WaitForSeconds(shakeTime);
        isShaking = false;
        transform.position = originalPos;
    }
}
