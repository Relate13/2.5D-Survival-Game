using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeUsage : Tool
{
    public GameObject AxeEntity;
    public Vector3 hitPos;
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
            ToolsWielder.getInstance().WieldTool(AxeEntity, CD, pos, 90);
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
            if (hitObj.TryGetComponent(out TreeEntity Hitted))
            {
                Hitted.Harm(damage);
            }
            else if (hitObj.TryGetComponent(out Decorative HittedDecoratives))
            {
                HittedDecoratives.Harm(damage);
            }
            else if(hitObj.TryGetComponent(out Crop HittedCrop))
            {
                HittedCrop.Harvest();
            }
        }
    }
}
