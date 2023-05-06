using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MobileOld : MonoBehaviour
{
    public Strategy strategy;
    public Vector3 destination;
    public GameObject followingObject;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        strategy = Strategy.IDLE;
        destination = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (strategy)
        {
            case Strategy.IDLE:
                destination = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
                destination += transform.position;
                strategy = Strategy.MOVING;
                Debug.Log("Mobile MOVING");
                break;
            case Strategy.MOVING:
                //rb.AddForce((destination-transform.position).normalized);
                transform.position = Vector3.MoveTowards(transform.position, destination, 1f * Time.deltaTime);
                if (Vector3.Distance(destination, transform.position) < 1.5)
                {
                    strategy = Strategy.IDLE;
                    Debug.Log("Mobile IDLE");
                }
                break;
            case Strategy.FOLLOWING:
                transform.position = Vector3.MoveTowards(transform.position, followingObject.transform.position, 1f * Time.deltaTime);
                //rb.AddForce((followingObject.transform.position - transform.position).normalized);
                if (Vector3.Distance(followingObject.transform.position, transform.position) > 10)
                {
                    strategy = Strategy.IDLE;
                    Debug.Log("Mobile IDLE");
                }
                break;
            default:break;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        destination = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
    }
    /*private void OnTriggerEnter(Collider other)
    {
        Plantable plantable;
        if(other.TryGetComponent(out plantable))
        {
            plantable.Harvest();
        }
    }*/
}
