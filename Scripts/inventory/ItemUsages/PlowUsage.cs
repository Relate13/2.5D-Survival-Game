using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlowUsage : Tool
{
    public GameObject PlowEntity;
    private Vector3 hitPos;
    public override int Operate(Vector3 pos)
    {
        bool firstUse = false;
        if (LaborTimer == null)
        {
            LaborTimer = new WorldTimer();
            firstUse = true;
        }
        if (firstUse || LaborTimer.Elapsed(CD))
        {
            GameDataHolder.getInstance().player.freezePlayer(CD);
            ToolsWielder.getInstance().WieldTool(PlowEntity, CD, pos, 90);
            hitPos = pos;
            Invoke("StartHit", CD);
            LaborTimer.Capture();
        }
        return 0;
    }
    public void StartHit()
    {
        if (TryRayHit(hitPos, out GameObject hitObj))
        {
            if (hitObj.TryGetComponent(out Grass Hitted))
            {
                Hitted.Harm(damage);
            }
            else if (hitObj.TryGetComponent(out Crop HittedCrop))
            {
                HittedCrop.Harm(damage);
            }
            else if (hitObj.TryGetComponent(out Bush HittedBush))
            {
                HittedBush.Harvest();
            }
        }
        else
        {
            MapBlock block = TileTerrain.GetInstance().GetMapBlock(hitPos);
            if (block.tileBase == 1)
            {
                TileTerrain.GetInstance().SetGameMapTile(hitPos, 3);
                AudioClip[] sounds = GameDataHolder.getInstance().plowSound;
                SoundManager.GetInstance().PlaySound(sounds[Random.Range(0, sounds.Length)]);
            }
        
        }
    }
}
