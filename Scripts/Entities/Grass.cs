using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : Plantable
{
    private void Awake()
    {
        Stage = 1;
    }
    public override EntityControlUnit GenerateECU(int mode)
    {
        EntityControlUnit ecu = new EntityControlUnit();
        ecu.entityId = EntityID;
        ecu.data = "";
        return ecu;
    }

    public override void Grow()
    {
        return;
    }

    public override void Harvest()
    {
        if (Random.Range(0f, 1f)<0.25)
        {
            GameObject trophy = Instantiate(trophies[0], transform.position, Quaternion.identity);
            trophy.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, 0)); 
        }
        AudioClip[] sounds = GameDataHolder.getInstance().CropSound;
        SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
        SelfDestruct();
    }

    public override void Init()
    {
        return;
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
