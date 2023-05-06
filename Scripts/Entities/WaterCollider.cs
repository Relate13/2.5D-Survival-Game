using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollider : Entity
{
    public override EntityControlUnit GenerateECU(int mode)
    {
        EntityControlUnit ecu = new EntityControlUnit();
        ecu.entityId = EntityID;
        return ecu;
    }

    public override void LoadData()
    {
        return;
    }

    public override void SaveData()
    {
        return;
    }
}
