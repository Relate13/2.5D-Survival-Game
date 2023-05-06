using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : Plantable
{
    public GameObject fruit;
    public override EntityControlUnit GenerateECU(int mode)
    {
        EntityControlUnit ecu = new EntityControlUnit();
        ecu.entityId = EntityID;
        ecu.data = "" + Random.Range(0, 2);
        return ecu;
    }

    public override void Grow()
    {
        ++status;
        if (status > 0)
        {
            fruit.SetActive(true);
        }
    }

    public override void Harvest()
    {
        if (status < 1)
        {
            GameObject trophy = Instantiate(trophies[0], transform.position, Quaternion.identity);
            trophy.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, 0));
            SelfDestruct();
        }
        else
        {
            GameObject trophy = Instantiate(trophies[1], transform.position, Quaternion.identity);
            trophy.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, 0));
            fruit.SetActive(false);
            status = 0;
            InvokeRepeating("TryGrow", Interval, Interval);
        }
    }
    public override void Init()
    {
        Stage = 2;
        if (status < 1)
            fruit.SetActive(false);
        else fruit.SetActive(true);
    }

    public override void LoadData()
    {
        status = int.Parse(selfECU.data);
        Init();
    }

    public override void SaveData()
    {
        selfECU.data = status + "";
    }
}
