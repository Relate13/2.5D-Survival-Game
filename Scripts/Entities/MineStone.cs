using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineStone : Harvestable
{

    public override void LoadData()
    {
        return;
    }

    public override void SaveData()
    {
        return;
    }
    public override EntityControlUnit GenerateECU(int mode)
    {
        EntityControlUnit ecu = new EntityControlUnit();
        ecu.entityId = EntityID;
        ecu.data = "";
        return ecu;
    }
    public override void harmEffect()
    {
        AudioClip[] sounds = GameDataHolder.getInstance().RockSound;
        SoundManager.GetInstance().PlaySound(sounds[Random.Range(0,sounds.Length)]);
        GetComponent<ShakeEffect>().ShakeMe();
    }
    public override void Harvest()
    {
        GenerateTrophies();
        AudioClip[] sounds = GameDataHolder.getInstance().RockSound;
        SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
        SelfDestruct();
    }
}
