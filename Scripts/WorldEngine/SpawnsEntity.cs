using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnsEntity : Usage
{
    public GameObject entitySpawns;
    public override int Operate(Vector3 pos)
    {
        Entity entity = entitySpawns.GetComponent<Entity>();
        if (TileTerrain.GetInstance().GenerateEntity(entity.EntityID, pos, 1))
        {
            if (entity is Decorative)
            {
                AudioClip[] sounds = GameDataHolder.getInstance().CommonSound;
                SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
            }
            else
            {
                AudioClip[] sounds = GameDataHolder.getInstance().plantSound;
                SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
            }
            return 1; 
        }
        else
        {
            MessageSystem.GetInstance().NewErrorMessage("无法在此地块放置该物品");
            return 0;
        }//generation failed
    }
}
