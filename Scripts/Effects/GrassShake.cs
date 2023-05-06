using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassShake : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 Rotation = new Vector3();
    public GameObject Grass;
    float counter = 0;
    public float Strength=1;
    public float Speed=0.01f;
    // Update is called once per frame
    private void Start()
    {
        Strength += Random.Range(-1f, 1f);
        Speed+=Random.Range(-0.01f, 0.01f);
    }
    void Update()
    {
        Rotation.z=Mathf.Sin(counter);
        Grass.transform.rotation = Quaternion.Euler(Rotation*Strength);
        //Debug.Log(transform.rotation);
        counter +=Speed;
        if (counter >= 2*Mathf.PI) 
            counter = 0;
    }
    void RecoverStatus()
    {
        Speed /= 3;
    }
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Triggered");
        Speed *= 3;
        Invoke("RecoverStatus", 0.5f);
    }
}
