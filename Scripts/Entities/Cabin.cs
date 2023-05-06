using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabin : Decorative
{
    WorldTimer restTimer;
    public override EntityControlUnit GenerateECU(int mode)
    {
        EntityControlUnit ecu = new EntityControlUnit();
        ecu.entityId = EntityID;
        ecu.data = "";
        return ecu;
    }

    public override void Harvest()
    {
        GameObject trophy = Instantiate(trophies[0], transform.position, Quaternion.identity);
        trophy.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, 0));
        SelfDestruct();
    }

    public override void LoadData()
    {
        return;
    }

    public override void SaveData()
    {
        return;
    }
    public void OnTriggerStay(Collider other)
    {
        if (restTimer == null)
        {
            restTimer = new WorldTimer();
        }
        if (restTimer.Elapsed(2f)) 
        {
            if (other.TryGetComponent(out Player p))
            {
                p.status.AddEnergy(5);
                StatusPanelController.GetInstance().SetStatus(p.status);
                restTimer.Capture();
            } 
        }
    }
}
